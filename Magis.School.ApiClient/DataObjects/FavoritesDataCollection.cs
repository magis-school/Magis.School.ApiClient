using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Magis.School.ApiClient.DataObjects.Base;
using Magis.School.ApiClient.DataObjects.Contexts;
using Magis.School.ApiClient.Endpoints;
using Magis.School.ApiClient.Events;
using Magis.School.ApiClient.Models;
using Magis.School.ApiClient.Models.Authorization;

namespace Magis.School.ApiClient.DataObjects
{
    public class FavoritesDataCollection : DataCollection<WebEndpoint, Favorite>
    {
        public FavoritesDataCollection(WebEndpoint sourceEndpoint, DataObjectContext context) : base(sourceEndpoint, context, UpdateEvent.AllFavoritesChanged,
            UpdateEvent.FavoriteChanged) { }

        protected override async Task<(ICollection<Favorite> collection, IDictionary<string, AccessAction> availableActions)> QueryCollectionAsync(
            string eventStreamId)
        {
            ICollection<Favorite> result = await SourceEndpoint.Favorites.GetFavoritesAsync(eventStreamId).ConfigureAwait(false);
            return (result, null);
        }

        protected override Task<Favorite> QueryCollectionItemAsync(string target, string eventStreamId)
        {
            return SourceEndpoint.Favorites.GetFavoriteAsync(target, eventStreamId, true);
        }

        protected override Favorite FindTargetInCollection(string target)
        {
            return Value.FirstOrDefault(i => i.Id == target);
        }
    }
}
