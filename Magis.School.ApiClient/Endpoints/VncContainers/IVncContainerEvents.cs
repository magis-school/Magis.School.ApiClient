using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Refit;

namespace Magis.School.ApiClient.Endpoints.VncContainers
{
    public interface IVncContainerEvents
    {
        [Get("/api/vnc-containers/events")]
        [Headers("Authorization: Basic")]
        Task<Stream> GetEventStreamAsync(CancellationToken cancellationToken = default);
    }
}
