using System.Threading;
using System.Threading.Tasks;
using Magis.School.ApiClient.Models;
using Magis.School.ApiClient.Models.Authorization;
using Refit;

namespace Magis.School.ApiClient.Endpoints.Web
{
    public interface IVncContainers
    {
        [Get("/api/web/vnc-containers")]
        Task<DataCollectionAndAccessResponse<VncContainer>> GetVncContainersAsync([Query] string eventStreamId = null, CancellationToken cancellationToken = default);

        [Get("/api/web/vnc-containers/{id}")]
        Task<DataAndAccessResponse<VncContainer>> GetVncContainerAsync(string id, [Query] string eventStreamId = null, [Query] bool checkExists = false,
            CancellationToken cancellationToken = default);
    }
}
