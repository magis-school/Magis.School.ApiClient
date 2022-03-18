using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Magis.School.ApiClient.Models;
using Magis.School.ApiClient.Models.Authorization;
using Refit;

namespace Magis.School.ApiClient.Endpoints.Web
{
    public interface IFavorites
    {
        [Get("/api/web/favorites")]
        Task<ICollection<Favorite>> GetFavoritesAsync([Query] string eventStreamId = null, CancellationToken cancellationToken = default);

        [Get("/api/web/favorites/{favoriteId}")]
        Task<Favorite> GetFavoriteAsync(string favoriteId, [Query] string eventStreamId = null, [Query] bool checkExists = false,
            CancellationToken cancellationToken = default);
    }
}
