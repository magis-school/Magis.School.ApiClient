using System.Threading;
using System.Threading.Tasks;
using Magis.School.ApiClient.Models;
using Refit;

namespace Magis.School.ApiClient.Endpoints.Web
{
    public interface IWebAuth
    {
        [Get("/api/web/auth")]
        Task<User> GetLoginAsync(CancellationToken cancellationToken = default);

        [Post("/api/web/auth/login")]
        Task<User> LoginAsync(UserCredentialsInput credentials, CancellationToken cancellationToken = default);

        [Post("/api/web/auth/logout")]
        Task LogoutAsync(CancellationToken cancellationToken = default);
    }
}
