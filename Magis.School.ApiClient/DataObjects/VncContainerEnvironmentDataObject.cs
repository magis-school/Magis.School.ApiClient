using System.Collections.Generic;
using System.Threading.Tasks;
using Magis.School.ApiClient.DataObjects.Base;
using Magis.School.ApiClient.DataObjects.Contexts;
using Magis.School.ApiClient.Endpoints;
using Magis.School.ApiClient.Events;
using Magis.School.ApiClient.Models;
using Magis.School.ApiClient.Models.Authorization;

namespace Magis.School.ApiClient.DataObjects
{
    public sealed class VncContainerEnvironmentDataObject : DataObject<VncContainersEndpoint, VncContainerEnvironment>
    {
        private bool _disposed;

        public VncContainerEnvironmentDataObject(VncContainersEndpoint endpoint, DefaultDataObjectContext context) : base(endpoint, context,
            UpdateEvent.VncContainerEnvironmentChanged) { }

        protected override async Task<(VncContainerEnvironment value, IDictionary<string, AccessAction> availableActions)> QueryValueAsync(string eventStreamId)
        {
            VncContainerEnvironment result = await SourceEndpoint.Status.GetEnvironmentAsync(eventStreamId).ConfigureAwait(false);
            return (result, null);
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            _disposed = true;
            base.Dispose(disposing);
        }
    }
}
