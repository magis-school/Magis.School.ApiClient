using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Magis.School.ApiClient.Events
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum UpdateEvent
    {
        AppChanged,
        AllAppsChanged,
        AppSourceChanged,
        AllAppSourcesChanged,
        ComputerChanged,
        AllComputersChanged,
        ConfigChanged,
        CourseChanged,
        AllCoursesChanged,
        CourseInfoChanged,
        AllCourseInfosChanged,
        CourseShareChanged,
        AllCourseSharesChanged,
        CourseSeatingPlanChanged,
        AllCourseSeatingPlansChanged,
        CourseAndRoomSeatingPlanChanged,
        AllCourseAndRoomSeatingPlansChanged,
        UserChanged,
        AllUsersChanged,
        LessonChanged,
        AllLessonsChanged,
        RoomChanged,
        AllRoomsChanged,
        NotificationChanged,
        AllNotificationsChanged,
        VncContainerChanged,
        AllVncContainersChanged,
        ComputerNetworkInfoChanged,
        ComputerSessionStatusChanged,
        VncContainerEnvironmentChanged,
        FileShareChanged,
        AllFileSharesChanged,
        FileShareForPathChanged,
        AllFileSharesForPathChanged
    }
}
