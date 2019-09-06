using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Refit;

namespace Magis.School.ApiClient.Endpoints.Computers
{
    public interface IComputerEvents
    {
        [Get("/api/computers/events")]
        [Headers("Authorization: Basic")]
        Task<Stream> GetEventStreamAsync(CancellationToken cancellationToken = default);
    }
}
