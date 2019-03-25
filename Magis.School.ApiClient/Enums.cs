using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Magis.School.ApiClient
{
    public static class Enums
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enum Role
        {
            Student,
            Employee
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum RoleGroup
        {
            Teachers,
            Admins
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum ComputerType
        {
            Equipment,
            LessonBased
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum ActiveSessionView
        {
            LoginNotAllowed,
            NoLessonStarted,
            NotMemberOfCourse,
            MemberOfCourseButNotParticipatingToLesson,
            Desktop,
            ScreenLocked
        }
    }
}
