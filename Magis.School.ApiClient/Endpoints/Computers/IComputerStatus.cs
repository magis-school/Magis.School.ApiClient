using System.Threading.Tasks;
using Magis.School.ApiClient.Models;
using Refit;

namespace Magis.School.ApiClient.Endpoints.Computers
{
    public interface IComputerStatus
    {
        [Get("/api/computers/status/computer")]
        [Headers("Authorization: Basic")]
        Task<Computer> GetComputerInfoAsync();

        [Get("/api/computers/status/network")]
        [Headers("Authorization: Basic")]
        Task<ComputerNetworkInfo> GetNetworkInfoAsync([Query] string eventStreamId = null);

        [Post("/api/computers/status/heartbeat")]
        [Headers("Authorization: Basic")]
        Task SendHeartbeatAsync([Body] ComputerHeartbeatInput heartbeatInput);

        [Post("/api/computers/status/initialize-session")]
        [Headers("Authorization: Basic")]
        Task<ComputerSessionStatus> InitializeSessionAsync([Body] InitializeSessionInput initializeSessionInput);

        [Get("/api/computers/status/session-status")]
        [Headers("Authorization: Basic")]
        Task<ComputerSessionStatus> GetSessionStatusAsync([Query] string eventStreamId = null);
    }
}
