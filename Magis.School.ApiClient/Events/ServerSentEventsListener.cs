using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Magis.School.ApiClient.Events.Messages;
using Newtonsoft.Json;
using ErrorEventArgs = Magis.School.ApiClient.Utils.ErrorEventArgs;

namespace Magis.School.ApiClient.Events
{
    internal class ServerSentEventsListener : IDisposable
    {
        private const string InitializationEventName = "EVENT-STREAM-ID";

        public event EventHandler<ListeningStartedEventArgs> ListeningStarted;
        public event EventHandler<EventArgs> ListeningStopped;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<ErrorEventArgs> ErrorOccured;


        private bool _listening = false;
        private CancellationTokenSource _listeningCts;

        private readonly SemaphoreSlim _listeningSemaphore = new SemaphoreSlim(1, 1);

        private bool _disposed = false;

        internal async Task StartListeningAsync(Stream eventStream, CancellationToken cancellationToken = default)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            if (!(eventStream ?? throw new ArgumentNullException(nameof(eventStream))).CanRead)
                throw new ArgumentException("Event-Stream is not readable.", nameof(eventStream));

            await _listeningSemaphore.WaitAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            try
            {
                if (_listening)
                    return;

                _listeningCts = new CancellationTokenSource();
                CancellationToken ct = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _listeningCts.Token).Token;

#pragma warning disable 4014
                ListenForEventsAsync(eventStream, ct).ContinueWith(continuationAction: async task => {
                    try
                    {
                        // Report error
                        if (task.IsFaulted && !(task.Exception?.InnerException is OperationCanceledException))
                            ErrorOccured?.Invoke(this, new ErrorEventArgs(task.Exception));

                        // Try to stop listening
                        await StopListeningAsync().ConfigureAwait(continueOnCapturedContext: false);
                    }
                    catch (Exception ex)
                    {
                        ErrorOccured?.Invoke(this, new ErrorEventArgs(ex));
                    }
                }, ct);
#pragma warning restore 4014
                _listening = true;
            }
            finally
            {
                _listeningSemaphore.Release();
            }
        }

        internal async Task StopListeningAsync()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            await _listeningSemaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                if (!_listening)
                    return;

                _listeningCts.Cancel();

                _listening = false;
                ListeningStopped?.Invoke(this, EventArgs.Empty);
            }
            finally
            {
                _listeningSemaphore.Release();
            }
        }

        private async Task ListenForEventsAsync(Stream eventStream, CancellationToken cancellationToken)
        {
            using (var streamReader = new StreamReader(eventStream))
            {
                async Task<(string key, string value)> ReadLineAsync()
                {
                    string line = await streamReader.ReadLineAsync().ConfigureAwait(continueOnCapturedContext: false);
                    if (string.IsNullOrWhiteSpace(line))
                        return (null, null);
                    string key = line.Substring(0, line.IndexOf(':'));
                    string value = line.Substring(key.Length + 1).TrimStart(' ');
                    return (key, value);
                }

                string eventName = null;
                var dataLines = new List<string>();

                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    (string key, string value) = await ReadLineAsync().ConfigureAwait(false);
                    cancellationToken.ThrowIfCancellationRequested();

                    switch (key)
                    {
                        case "event":
                            eventName = value;
                            break;
                        case "data":
                            dataLines.Add(value);
                            break;
                        case null:
                        case "":
                            if (eventName != null && dataLines.Any())
                                HandleEventReceived(eventName, string.Join(Environment.NewLine, dataLines));
                            eventName = null;
                            dataLines = new List<string>();
                            break;
                    }
                }
            }
        }

        private void HandleEventReceived(string eventName, string data)
        {
            _listeningCts.Token.ThrowIfCancellationRequested();

            try
            {
                IMessage message;
                switch (eventName)
                {
                    case InitializationEventName:
                        ListeningStarted?.Invoke(this, new ListeningStartedEventArgs(data));
                        return;
                    case nameof(UpdateMessage):
                        message = JsonConvert.DeserializeObject<UpdateMessage>(data);
                        break;
                    default:
                        return;
                }

                MessageReceived?.Invoke(this, new MessageReceivedEventArgs(message));
            }
            catch (Exception ex)
            {
                // Ignore invalid events
                ErrorOccured?.Invoke(this, new ErrorEventArgs(ex));
            }
        }

        public void Dispose()
        {
            _listeningCts?.Cancel();

            _listeningCts?.Dispose();
            _listeningSemaphore?.Dispose();
            _disposed = true;
        }
    }
}
