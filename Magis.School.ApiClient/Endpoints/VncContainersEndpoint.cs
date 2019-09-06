using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Magis.School.ApiClient.DataObjects;
using Magis.School.ApiClient.DataObjects.Caching;
using Magis.School.ApiClient.DataObjects.Contexts;
using Magis.School.ApiClient.Endpoints.Computers;
using Magis.School.ApiClient.Endpoints.EndpointBase;
using Magis.School.ApiClient.Endpoints.VncContainers;
using Magis.School.ApiClient.Events.Messages;
using Magis.School.ApiClient.Utils;
using Newtonsoft.Json;
using Refit;

namespace Magis.School.ApiClient.Endpoints
{
    public sealed class VncContainersEndpoint : EndpointWithEvents
    {
        public IVncContainerStatus Status { get; }

        public IVncContainerEvents Events { get; }

        private readonly DataObjectCache _dataObjectCache = new DataObjectCache();

        private bool _disposed;

        internal VncContainersEndpoint(RefitSettings refitSettings, string serverBackendUrl, string vncContainerName, string apiToken)
        {
            var httpClient = new HttpClient(new ApiHttpClientHandler(vncContainerName, apiToken)) {BaseAddress = new Uri(serverBackendUrl)};

            Status = RestService.For<IVncContainerStatus>(httpClient, refitSettings);
            Events = RestService.For<IVncContainerEvents>(httpClient, refitSettings);
        }

        public VncContainerEnvironmentDataObject GetVncContainerEnvironment()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            return _dataObjectCache.GetOrAddDataObject(new DefaultDataObjectContext(), context => new VncContainerEnvironmentDataObject(this, context));
        }

        protected override Task<Stream> QueryEventStreamAsync(CancellationToken cancellationToken = default) => Events.GetEventStreamAsync(cancellationToken);

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
                _dataObjectCache.Dispose();

            _disposed = true;
            base.Dispose(disposing);
        }
    }
}
