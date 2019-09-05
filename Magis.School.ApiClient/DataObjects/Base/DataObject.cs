using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Magis.School.ApiClient.DataObjects.Contexts;
using Magis.School.ApiClient.Endpoints;
using Magis.School.ApiClient.Endpoints.EndpointBase;
using Magis.School.ApiClient.Events;
using Magis.School.ApiClient.Models.Authorization;
using Magis.School.ApiClient.Utils;

namespace Magis.School.ApiClient.DataObjects.Base
{
    public abstract class DataObject<TSourceEndpoint, TValue> : IDataObject where TSourceEndpoint : EndpointWithEvents
    {
        public event EventHandler<ErrorEventArgs> UpdateErrorOccured;

        public event EventHandler<EventArgs> ValueUpdated;

        protected delegate Task UpdateEventHandlerDelegate(string target = null);

        public TSourceEndpoint SourceEndpoint { get; }

        public DataObjectContext Context { get; }

        public bool Loading { get; private set; }

        public bool Loaded { get; private set; }

        public IDictionary<string, AccessAction> AvailableActions { get; private set; }

        public TValue Value { get; private set; }

        public SemaphoreSlim ValueSemaphore { get; } = new SemaphoreSlim(1, 1);

        protected IDictionary<UpdateEvent, UpdateEventHandlerDelegate> UpdateEventHandlers { get; } = new Dictionary<UpdateEvent, UpdateEventHandlerDelegate>();

        private TaskCompletionSource<object> _loadingCompletionSource;

        private bool _disposed;

        internal DataObject(TSourceEndpoint sourceEndpoint, DataObjectContext context, UpdateEvent valueChangedEvent)
        {
            SourceEndpoint = sourceEndpoint ?? throw new ArgumentNullException(nameof(sourceEndpoint));
            Context = context ?? throw new ArgumentNullException(nameof(context));

            UpdateEventHandlers.Add(valueChangedEvent, HandleValueChangedAsync);

            SourceEndpoint.EventListeningStateChanged += OnEventListeningStateChanged;
            SourceEndpoint.DataUpdatedReceived += OnDataUpdatedReceived;
        }

        public async Task EnsureLoadedAsync()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            TaskCompletionSource<object> currentCompletionSource = _loadingCompletionSource;
            if (Loading && currentCompletionSource != null)
                await currentCompletionSource.Task.ConfigureAwait(false);
            else if (!Loaded)
                await LoadAsync().ConfigureAwait(false);
        }

        public async Task ReloadAsync()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            TaskCompletionSource<object> currentCompletionSource = _loadingCompletionSource;
            if (Loading && currentCompletionSource != null)
                await currentCompletionSource.Task.ConfigureAwait(false);
            else
                await LoadAsync().ConfigureAwait(false);
        }

        public async Task ResetAsync()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            await ValueSemaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                Value = default;
                AvailableActions = null;
                Loaded = false;
            }
            finally
            {
                ValueSemaphore.Release();
            }
        }

        protected abstract Task<(TValue value, IDictionary<string, AccessAction> availableActions)> QueryValueAsync(string eventStreamId);

        private async Task LoadAsync()
        {
            // Create a completion source to enable other methods to wait for loading completion
            _loadingCompletionSource = new TaskCompletionSource<object>();
            Loading = true;
            try
            {
                // Ensure the event stream is active
                await SourceEndpoint.EnsureListeningForEventsAsync().ConfigureAwait(false);

                // Update value
                await UpdateValueAsync().ConfigureAwait(false);

                Loading = false;
                _loadingCompletionSource.SetResult(new object());
            }
            catch (Exception ex)
            {
                Loading = false;
                _loadingCompletionSource.SetException(ex);
            }
        }

        private async Task UpdateValueAsync()
        {
            (TValue value, IDictionary<string, AccessAction> availableActions) = await QueryValueAsync(SourceEndpoint.EventStreamId).ConfigureAwait(false);

            await ValueSemaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                Value = value;
                AvailableActions = availableActions ?? new Dictionary<string, AccessAction>();
                Loaded = true;
                ValueUpdated?.Invoke(this, EventArgs.Empty);
            }
            finally
            {
                ValueSemaphore.Release();
            }
        }

        private async void OnEventListeningStateChanged(object sender, EventListeningStateChangedEventArgs e)
        {
            if (!Loaded || e.State != EventListeningState.Started)
                return;

            try
            {
                // Reload data to ensure it's up to date
                await ReloadAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                UpdateErrorOccured?.Invoke(this, new ErrorEventArgs(ex));
            }
        }

        private async void OnDataUpdatedReceived(object sender, DataUpdatedReceivedEventArgs e)
        {
            if (!Loaded || Loading)
                return;

            try
            {
                if (!UpdateEventHandlers.ContainsKey(e.UpdateEvent))
                    return;
                if (!Context.Matches(e.Context))
                    return;
                await UpdateEventHandlers[e.UpdateEvent].Invoke(e.Target).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                UpdateErrorOccured?.Invoke(this, new ErrorEventArgs(ex));
            }
        }

        private Task HandleValueChangedAsync(string target)
        {
            return UpdateValueAsync();
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
                SourceEndpoint.EventListeningStateChanged -= OnEventListeningStateChanged;
                SourceEndpoint.DataUpdatedReceived -= OnDataUpdatedReceived;
                ValueSemaphore?.Dispose();
            }

            _disposed = true;
        }
    }
}
