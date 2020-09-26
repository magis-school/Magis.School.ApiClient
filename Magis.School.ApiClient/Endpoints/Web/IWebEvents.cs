using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Refit;

namespace Magis.School.ApiClient.Endpoints.Web
{
    public interface IWebEvents
    {
        [Get("/api/web/events")]
        Task<Stream> GetEventStreamAsync(CancellationToken cancellationToken = default);
    }
}
