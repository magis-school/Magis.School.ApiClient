using System.Threading;
using System.Threading.Tasks;
using Magis.School.ApiClient.Models;
using Refit;

namespace Magis.School.ApiClient.Endpoints.VncContainers
{
    public interface IVncContainerStatus
    {
        [Get("/api/vnc-containers/status/environment")]
        [Headers("Authorization: Basic")]
        Task<VncContainerEnvironment> GetEnvironmentAsync([Query] string eventStreamId = null, CancellationToken cancellationToken = default);

        [Post("/api/vnc-containers/status/set-in-use")]
        [Headers("Authorization: Basic")]
        Task<VncContainer> SetInUseAsync([Body] SetInUseInput setInUseInput, CancellationToken cancellationToken = default);
    }
}
