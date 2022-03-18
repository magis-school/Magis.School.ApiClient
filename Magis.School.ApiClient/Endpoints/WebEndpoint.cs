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
using Magis.School.ApiClient.Endpoints.Web;
using Magis.School.ApiClient.Utils;
using Refit;

namespace Magis.School.ApiClient.Endpoints
{
    public sealed class WebEndpoint : EndpointWithEvents
    {
        public IWebAuth Auth { get; }

        public IApps Apps { get; }

        public ICourses Courses { get; }

        public IVncContainers VncContainers { get; }

        public IWebEvents Events { get; }

        private readonly DataObjectCache _dataObjectCache = new DataObjectCache();

        private bool _disposed;

        internal WebEndpoint(RefitSettings refitSettings, string serverBackendUrl)
        {
            var httpClient = new HttpClient(new ApiHttpClientHandler()) {BaseAddress = new Uri(serverBackendUrl)};

            Auth = RestService.For<IWebAuth>(httpClient, refitSettings);
            Apps = RestService.For<IApps>(httpClient, refitSettings);
            Courses = RestService.For<ICourses>(httpClient, refitSettings);
            VncContainers = RestService.For<IVncContainers>(httpClient, refitSettings);
            Events = RestService.For<IWebEvents>(httpClient, refitSettings);
        }

        public AppsDataCollection GetApps()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            return _dataObjectCache.GetOrAddDataObject(new DefaultDataObjectContext(), context => new AppsDataCollection(this, context));
        }

        public CoursesDataCollection GetCourses()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            return _dataObjectCache.GetOrAddDataObject(new DefaultDataObjectContext(), context => new CoursesDataCollection(this, context));
        }

        public VncContainersDataCollection GetVncContainers()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            // The used endpoint allways incldues internal containers.
            return _dataObjectCache.GetOrAddDataObject(new VncContainersDataObjectContext(true), context => new VncContainersDataCollection(this, context));
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
