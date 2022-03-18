using System.Threading;
using System.Threading.Tasks;
using Magis.School.ApiClient.Models;
using Magis.School.ApiClient.Models.Authorization;
using Refit;

namespace Magis.School.ApiClient.Endpoints.Web
{
    public interface ICourses
    {
        [Get("/api/web/courses")]
        Task<DataCollectionAndAccessResponse<Course>> GetCoursesAsync([Query] string eventStreamId = null, CancellationToken cancellationToken = default);

        [Get("/api/web/courses/{courseName}")]
        Task<DataAndAccessResponse<Course>> GetCourseAsync(string courseName, [Query] string eventStreamId = null, [Query] bool checkExists = false,
            CancellationToken cancellationToken = default);
    }
}
