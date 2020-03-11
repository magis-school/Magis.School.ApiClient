using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Magis.School.ApiClient.Events;
using Magis.School.ApiClient.Events.Messages;
using Refit;
using ErrorEventArgs = Magis.School.ApiClient.Utils.ErrorEventArgs;

namespace Magis.School.ApiClient.Endpoints.EndpointBase
{
    public abstract class EndpointWithEvents : Endpoint, IDisposable
    {
        private const int PauseBeforeReconnect = 3000;

        public event EventHandler<EventListeningStateChangedEventArgs> EventListeningStateChanged;

        public event EventHandler<ErrorEventArgs> EventListeningErrorOccured;

        public event EventHandler<DataUpdatedReceivedEventArgs> DataUpdatedReceived;

        public EventListeningState CurrentEventListeningState
        {
            get => _currentEventListeningState;
            private set
            {
                if (_currentEventListeningState == value)
                    return;
                _currentEventListeningState = value;
                EventListeningStateChanged?.Invoke(this, new EventListeningStateChangedEventArgs(value));
            }
        }

        public string EventStreamId { get; private set; }

        private readonly ServerSentEventsListener _sseListener;

        private bool _shouldBeListening;
        private readonly SemaphoreSlim _listeningControlSemaphore = new SemaphoreSlim(1, 1);
        private CancellationTokenSource _stopListeningCts;
        private EventListeningState _currentEventListeningState = EventListeningState.Stopped;

        private bool _disposed;

        internal EndpointWithEvents()
        {
            _sseListener = new ServerSentEventsListener();
            _sseListener.ListeningStarted += OnListeningStarted;
            _sseListener.ListeningStopped += OnListeningStopped;
            _sseListener.ErrorOccured += OnListeningErrorOccured;
            _sseListener.MessageReceived += OnMessageReceived;
        }

        public async Task EnsureListeningForEventsAsync(CancellationToken cancellationToken = default)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            // Locks until a listening-stop or a reconnect has finished
            await _listeningControlSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                if (_shouldBeListening)
                    return;

                _stopListeningCts = new CancellationTokenSource();
                CancellationToken ct = CancellationTokenSource.CreateLinkedTokenSource(_stopListeningCts.Token, cancellationToken).Token;

                await StartListeningWithRetriesAsync(ct).ConfigureAwait(false);
                _shouldBeListening = true;
            }
            finally
            {
                _listeningControlSemaphore.Release();
            }
        }

        public async Task StopListeningForEventsAsync()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            _stopListeningCts.Cancel();

            // Locks until all previous tasks have been canceled
            await _listeningControlSemaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                if (!_shouldBeListening)
                    throw new InvalidOperationException("Endpoint is not listening for events.");

                await _sseListener.StopListeningAsync().ConfigureAwait(false);
                _shouldBeListening = false;

                EventStreamId = null;
                CurrentEventListeningState = EventListeningState.Stopped;
            }
            finally
            {
                _listeningControlSemaphore.Release();
            }
        }

        protected abstract Task<Stream> QueryEventStreamAsync(CancellationToken cancellationToken = default);

        protected virtual Task HandleEventMessageAsync(IMessage message)
        {
            switch (message)
            {
                case UpdateMessage updateMessage:
                    DataUpdatedReceived?.Invoke(this, new DataUpdatedReceivedEventArgs(updateMessage.Event, updateMessage.Context ?? new Dictionary<string, object>(), updateMessage.Target));
                    break;
            }

            return Task.CompletedTask;
        }

        private void OnListeningStarted(object sender, ListeningStartedEventArgs e)
        {
            EventStreamId = e.EventStreamId;
            CurrentEventListeningState = EventListeningState.Started;
        }

        private async void OnListeningStopped(object sender, EventArgs e)
        {
            try
            {
                // Locks until the control-methods were completely executed because the ListeningStopped event might fire in the middle of those
                await _listeningControlSemaphore.WaitAsync().ConfigureAwait(false);
                try
                {
                    // Has the listening been interrupted?
                    if (_shouldBeListening && !_stopListeningCts.IsCancellationRequested)
                    {
                        EventStreamId = null;
                        CurrentEventListeningState = EventListeningState.Restarting;

                        // Wait before reconnect
                        await Task.Delay(PauseBeforeReconnect, _stopListeningCts.Token).ConfigureAwait(false);

                        // Try a reconnect
                        await StartListeningWithRetriesAsync(_stopListeningCts.Token).ConfigureAwait(false);
                    }
                }
                finally
                {
                    _listeningControlSemaphore.Release();
                }
            }
            catch
            {
                // Ignore exceptions here. Will only occur when the locking failed.
            }
        }

        private void OnListeningErrorOccured(object sender, ErrorEventArgs e)
        {
            EventListeningErrorOccured?.Invoke(sender, e);
        }

        private async void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            try
            {
                await HandleEventMessageAsync(e.Message).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                EventListeningErrorOccured?.Invoke(this, new ErrorEventArgs(ex));
            }
        }

        private async Task StartListeningWithRetriesAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // Query new event stream
                    Stream eventStream = await QueryEventStreamAsync(cancellationToken).ConfigureAwait(false);

                    // Restart listening
                    await _sseListener.StartListeningAsync(eventStream, cancellationToken).ConfigureAwait(false);

                    // Restart successful. Stop reconnecting
                    break;
                }
                catch (OperationCanceledException)
                {
                    // Stop reconnecting
                    break;
                }
                catch (Exception ex)
                {
                    // Catch and report error to ensure a reconnect can happen
                    EventListeningErrorOccured?.Invoke(this, new ErrorEventArgs(ex));
                }

                EventStreamId = null;
                CurrentEventListeningState = EventListeningState.Restarting;

                // Wait before reconnect
                await Task.Delay(PauseBeforeReconnect, cancellationToken).ConfigureAwait(false);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _stopListeningCts?.Cancel();

                _sseListener?.Dispose();
                _listeningControlSemaphore?.Dispose();
                _stopListeningCts?.Dispose();
            }

            _disposed = true;
        }
    }
}
