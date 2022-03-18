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
    public sealed class VncContainersDataCollection : DataCollection<WebEndpoint, DataAndAccessResponse<VncContainer>>
    {
        /// <inheritdoc />
        public VncContainersDataCollection(WebEndpoint sourceEndpoint, VncContainersDataObjectContext context) : base(
            sourceEndpoint, context, UpdateEvent.AllVncContainersChanged, UpdateEvent.VncContainerChanged) { }

        /// <inheritdoc />
        protected override async Task<(ICollection<DataAndAccessResponse<VncContainer>> collection, IDictionary<string, AccessAction> availableActions)> QueryCollectionAsync(
            string eventStreamId)
        {
            DataCollectionAndAccessResponse<VncContainer> result = await SourceEndpoint.VncContainers.GetVncContainersAsync(eventStreamId).ConfigureAwait(false);
            return (result.Entries, result.AvailableActions);
        }

        /// <inheritdoc />
        protected override Task<DataAndAccessResponse<VncContainer>> QueryCollectionItemAsync(string target)
        {
            return SourceEndpoint.VncContainers.GetVncContainerAsync(target, null, true);
        }

        /// <inheritdoc />
        protected override DataAndAccessResponse<VncContainer> FindTargetInCollection(string target) => Value.FirstOrDefault(i => i.Entry.Id == target);
    }
}
