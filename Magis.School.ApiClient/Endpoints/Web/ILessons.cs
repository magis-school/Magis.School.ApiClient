using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Magis.School.ApiClient.Models;
using Magis.School.ApiClient.Models.Authorization;
using Refit;

namespace Magis.School.ApiClient.Endpoints.Web
{
    public interface ILessons
    {
        [Get("/api/web/lessons")]
        Task<DataCollectionAndAccessResponse<Lesson>> GetLessonsAsync([Query] string eventStreamId = null, CancellationToken cancellationToken = default);

        [Get("/api/web/lessons/{courseName}")]
        Task<DataAndAccessResponse<Lesson>> GetLessonAsync(string courseName, [Query] string eventStreamId = null, [Query] bool checkExists = false,
            CancellationToken cancellationToken = default);
    }
}
