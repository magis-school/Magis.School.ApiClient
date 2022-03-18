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
    public class LessonsDataCollection : DataCollection<WebEndpoint, DataAndAccessResponse<Lesson>>
    {
        public LessonsDataCollection(WebEndpoint sourceEndpoint, DataObjectContext context) : base(sourceEndpoint, context, UpdateEvent.AllLessonsChanged,
            UpdateEvent.LessonChanged) { }

        protected override async Task<(ICollection<DataAndAccessResponse<Lesson>> collection, IDictionary<string, AccessAction> availableActions)>
            QueryCollectionAsync(string eventStreamId)
        {
            DataCollectionAndAccessResponse<Lesson> result = await SourceEndpoint.Lessons.GetLessonsAsync(eventStreamId).ConfigureAwait(false);
            return (result.Entries, result.AvailableActions);
        }

        protected override Task<DataAndAccessResponse<Lesson>> QueryCollectionItemAsync(string target)
        {
            return SourceEndpoint.Lessons.GetLessonAsync(target, null, true);
        }

        protected override DataAndAccessResponse<Lesson> FindTargetInCollection(string target) => Value.FirstOrDefault(i => i.Entry.CourseName == target);
    }
}
