using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Magis.School.ApiClient.DataObjects.Caching;
using Magis.School.ApiClient.Endpoints.Computers;
using Magis.School.ApiClient.Endpoints.EndpointBase;
using Magis.School.ApiClient.Events.Messages;
using Magis.School.ApiClient.Utils;
using Newtonsoft.Json;
using Refit;

namespace Magis.School.ApiClient.Endpoints
{
    public sealed class ComputersEndpoint : EndpointWithEvents
    {
        public IComputerStatus Status { get; }

        public IComputerEvents Events { get; }

        private readonly DataObjectCache _dataObjectCache = new DataObjectCache();

        private bool _disposed;

        internal ComputersEndpoint(RefitSettings refitSettings, string serverBackendUrl, string computerToken)
        {
            var httpClient = new HttpClient(new BasicAuthApiHttpClientHandler("computer", computerToken)) {BaseAddress = new Uri(serverBackendUrl)};

            Status = RestService.For<IComputerStatus>(httpClient, refitSettings);
            Events = RestService.For<IComputerEvents>(httpClient, refitSettings);
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
