using System.IO;
using System.Threading.Tasks;
using Refit;

namespace Magis.School.ApiClient.Endpoints.Computers
{
    internal interface IComputerEvents
    {
        [Get("/api/computers/events")]
        [Headers("Authorization: Basic")]
        Task<Stream> GetEventStreamAsync();
    }
}
