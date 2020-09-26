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
    public sealed class AppsDataCollection : DataCollection<WebEndpoint, DataAndAccessResponse<App>>
    {
        /// <inheritdoc />
        public AppsDataCollection(WebEndpoint sourceEndpoint, DefaultDataObjectContext context) : base(
            sourceEndpoint, context, UpdateEvent.AllAppsChanged, UpdateEvent.AppChanged) { }

        /// <inheritdoc />
        protected override async Task<(ICollection<DataAndAccessResponse<App>> collection, IDictionary<string, AccessAction> availableActions)> QueryCollectionAsync(
            string eventStreamId)
        {
            DataCollectionAndAccessResponse<App> result = await SourceEndpoint.Apps.GetAppsAsync(eventStreamId).ConfigureAwait(false);
            return (result.Entries, result.AvailableActions);
        }

        /// <inheritdoc />
        protected override Task<DataAndAccessResponse<App>> QueryCollectionItemAsync(string target, string eventStreamId)
        {
            return SourceEndpoint.Apps.GetAppAsync(target, eventStreamId, true);
        }

        /// <inheritdoc />
        protected override DataAndAccessResponse<App> FindTargetInCollection(string target) => Value.FirstOrDefault(i => i.Entry.Name == target);
    }
}
