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
        public enum LessonPhase
        {
            Work,
            Listen,
            TeacherDemo
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum ActiveComputerSessionView
        {
            LoginNotAllowed,
            NoLessonStarted,
            NotMemberOfCourse,
            Desktop,
            ScreenLocked
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum ActiveVncContainerView
        {
            Desktop,
            ScreenLocked
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum VncContainerAccessLevel
        {
            Owner,
            User,
            ReadOnly
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum VncContainerAccessorType
        {
            User
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum VncContainerStatus
        {
            Registered,
            Starting,
            Idle,
            InUse,
            Paused,
            Stopping
        }
    }
}
