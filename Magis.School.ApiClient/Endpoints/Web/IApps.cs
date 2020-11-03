using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Magis.School.ApiClient.Models;
using Magis.School.ApiClient.Models.Authorization;
using Refit;

namespace Magis.School.ApiClient.Endpoints.Web
{
    public interface IApps
    {
        [Get("/api/web/apps")]
        Task<DataCollectionAndAccessResponse<App>> GetAppsAsync([Query] string eventStreamId = null, CancellationToken cancellationToken = default);

        [Get("/api/web/apps/{appName}")]
        Task<DataAndAccessResponse<App>> GetAppAsync(string appName, [Query] string eventStreamId = null, [Query] bool checkExists = false,
            CancellationToken cancellationToken = default);

        // TODO: Because of a bug in the server (backend#60) the api call fails when no empty json object is sent.
        [Post("/api/web/apps/{appName}/start")]
        Task<string> StartAppAsync(string appName, [Body] Dictionary<string,string> empty);
    }
}
