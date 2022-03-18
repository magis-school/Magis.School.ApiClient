using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Magis.School.ApiClient.DataObjects.Contexts;
using Magis.School.ApiClient.Endpoints.EndpointBase;
using Magis.School.ApiClient.Events;
using Magis.School.ApiClient.Models.Authorization;

namespace Magis.School.ApiClient.DataObjects.Base
{
    public abstract class DataCollection<TSourceEndpoint, TItem> : DataObject<TSourceEndpoint, ObservableCollection<TItem>> where TSourceEndpoint : EndpointWithEvents
    {
        private bool _disposed;

        internal DataCollection(TSourceEndpoint sourceEndpoint, DataObjectContext context, UpdateEvent collectionChangedEvent, UpdateEvent collectionItemChangedEvent) : base(
            sourceEndpoint, context, collectionChangedEvent)
        {
            UpdateEventHandlers.Add(collectionItemChangedEvent, HandleCollectionItemChangedAsync);
        }

        protected abstract Task<(ICollection<TItem> collection, IDictionary<string, AccessAction> availableActions)> QueryCollectionAsync(string eventStreamId);

        protected abstract Task<TItem> QueryCollectionItemAsync(string target);

        protected abstract TItem FindTargetInCollection(string target);

        protected override async Task<(ObservableCollection<TItem> value, IDictionary<string, AccessAction> availableActions)> QueryValueAsync(string eventStreamId)
        {
            (ICollection<TItem> collection, IDictionary<string, AccessAction> availableActions) = await QueryCollectionAsync(eventStreamId).ConfigureAwait(false);
            return (new ObservableCollection<TItem>(collection), availableActions);
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            _disposed = true;
            base.Dispose(disposing);
        }

        private async Task HandleCollectionItemChangedAsync(string target)
        {
            // Query changed item
            TItem item = await QueryCollectionItemAsync(target).ConfigureAwait(false);

            await ValueSemaphore.WaitAsync(UpdatingCancellationToken).ConfigureAwait(false);
            try
            {
                // Ensure the data is still loaded
                if (!Loaded)
                    return;

                // Update collection
                TItem existingItem = FindTargetInCollection(target);
                int existingItemIndex = existingItem != null ? Value.IndexOf(existingItem) : -1;
                if (existingItemIndex >= 0 && item != null)
                    Value[existingItemIndex] = item;
                else if (existingItemIndex >= 0)
                    Value.RemoveAt(existingItemIndex);
                else if (item != null)
                    Value.Add(item);
            }
            finally
            {
                ValueSemaphore.Release();
            }
        }
    }
}
