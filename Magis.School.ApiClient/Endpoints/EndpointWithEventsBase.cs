using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Magis.School.ApiClient.Events;
using Magis.School.ApiClient.Events.Messages;
using ErrorEventArgs = Magis.School.ApiClient.Utils.ErrorEventArgs;

namespace Magis.School.ApiClient.Endpoints
{
    public abstract class EndpointWithEventsBase : EndpointBase, IDisposable
    {
        public enum EventListeningState
        {
            Stopped,
            Started,
            Restarting
        }

        public event EventHandler<EventArgs> EventListeningStateChanged;
        public event EventHandler<ErrorEventArgs> EventListeningErrorOccured;

        protected delegate Task<Stream> QueryEventStreamAsyncDelegate();

        public EventListeningState CurrentEventListeningState
        {
            get => _currentEventListeningState;
            private set
            {
                if (_currentEventListeningState == value)
                    return;
                _currentEventListeningState = value;
                EventListeningStateChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private readonly ServerSentEventsListener _sseListener;

        private bool _shouldBeListening = false;
        private QueryEventStreamAsyncDelegate _queryReconnectEventStreamDelegate = null;
        private readonly SemaphoreSlim _listeningControlSemaphore = new SemaphoreSlim(1, 1);
        private CancellationTokenSource _stopListeningCts = null;
        private EventListeningState _currentEventListeningState = EventListeningState.Stopped;

        private bool _disposed = false;

        internal EndpointWithEventsBase()
        {
            _sseListener = new ServerSentEventsListener();
            _sseListener.ListeningStarted += OnListeningStarted;
            _sseListener.ListeningStopped += OnListeningStopped;
            _sseListener.ErrorOccured += OnListeningErrorOccured;
            _sseListener.MessageReceived += OnMessageReceived;
        }

        protected async Task StartListeningForEventsInternalAsync(QueryEventStreamAsyncDelegate queryEventStreamDelegate)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(EndpointWithEventsBase));
            if (queryEventStreamDelegate == null)
                throw new ArgumentNullException(nameof(queryEventStreamDelegate));

            // Locks until a listening-stop or a reconnect has finished
            await _listeningControlSemaphore.WaitAsync().ConfigureAwait(continueOnCapturedContext: false);
            try
            {
                if (_shouldBeListening)
                    throw new InvalidOperationException("Endpoint is already listening for events.");

                _queryReconnectEventStreamDelegate = queryEventStreamDelegate;
                _stopListeningCts = new CancellationTokenSource();

                await StartListeningWithRetriesAsync(queryEventStreamDelegate, _stopListeningCts.Token).ConfigureAwait(continueOnCapturedContext: false);
                _shouldBeListening = true;
            }
            finally
            {
                _listeningControlSemaphore.Release();
            }
        }

        protected async Task StopListeningForEventsInternalAsync()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(EndpointWithEventsBase));

            _stopListeningCts.Cancel();

            // Locks until all previous tasks have been canceled
            await _listeningControlSemaphore.WaitAsync().ConfigureAwait(continueOnCapturedContext: false);
            try
            {
                if (!_shouldBeListening)
                    throw new InvalidOperationException("Endpoint is not listening for events.");

                await _sseListener.StopListeningAsync().ConfigureAwait(continueOnCapturedContext: false);
                _shouldBeListening = false;

                CurrentEventListeningState = EventListeningState.Stopped;
            }
            finally
            {
                _listeningControlSemaphore.Release();
            }
        }

        protected internal abstract Task HandleEventMessageAsync(IMessage message);

        private void OnListeningStarted(object sender, EventArgs e)
        {
            CurrentEventListeningState = EventListeningState.Started;
        }

        private async void OnListeningStopped(object sender, EventArgs e)
        {
            try
            {
                // Locks until the control-methods were completely executed because the ListeningStopped event might fire in the middle of those
                await _listeningControlSemaphore.WaitAsync().ConfigureAwait(continueOnCapturedContext: false);
                try
                {
                    // Has the listening been interrupted?
                    if (_shouldBeListening && !_stopListeningCts.IsCancellationRequested)
                        await StartListeningWithRetriesAsync(_queryReconnectEventStreamDelegate, _stopListeningCts.Token).ConfigureAwait(continueOnCapturedContext: false);
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
                await HandleEventMessageAsync(e.Message).ConfigureAwait(continueOnCapturedContext: false);
            }
            catch(Exception ex)
            {
                EventListeningErrorOccured?.Invoke(this,new ErrorEventArgs(ex));
            }
        }

        private async Task StartListeningWithRetriesAsync(QueryEventStreamAsyncDelegate queryEventStreamDelegate, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // Query new event stream
                    Stream eventStream = await _queryReconnectEventStreamDelegate.Invoke().ConfigureAwait(continueOnCapturedContext: false);

                    // Restart listening
                    await _sseListener.StartListeningAsync(eventStream).ConfigureAwait(continueOnCapturedContext: false);

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
                    EventListeningErrorOccured?.Invoke(this, new ErrorEventArgs(ex));
                }

                // Wait before reconnect
                CurrentEventListeningState = EventListeningState.Restarting;
                await Task.Delay(3000, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            }
        }

        public void Dispose()
        {
            _stopListeningCts?.Cancel();

            _sseListener?.Dispose();
            _listeningControlSemaphore?.Dispose();
            _stopListeningCts?.Dispose();
            _disposed = true;
        }
    }
}
