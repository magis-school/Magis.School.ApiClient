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
    public sealed class CoursesDataCollection : DataCollection<WebEndpoint, DataAndAccessResponse<Course>>
    {
        public CoursesDataCollection(WebEndpoint sourceEndpoint, DefaultDataObjectContext context) : base(sourceEndpoint, context, UpdateEvent.AllCoursesChanged,
            UpdateEvent.CourseChanged) { }

        protected override async Task<(ICollection<DataAndAccessResponse<Course>> collection, IDictionary<string, AccessAction> availableActions)> QueryCollectionAsync(
            string eventStreamId)
        {
            DataCollectionAndAccessResponse<Course> result = await SourceEndpoint.Courses.GetCoursesAsync(eventStreamId).ConfigureAwait(false);
            return (result.Entries, result.AvailableActions);
        }

        protected override Task<DataAndAccessResponse<Course>> QueryCollectionItemAsync(string target)
        {
            return SourceEndpoint.Courses.GetCourseAsync(target, null, true);
        }

        protected override DataAndAccessResponse<Course> FindTargetInCollection(string target) => Value.FirstOrDefault(i => i.Entry.Name == target);
    }
}
