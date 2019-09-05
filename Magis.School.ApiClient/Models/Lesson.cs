using System;
using System.Collections.Generic;

namespace Magis.School.ApiClient.Models
{
    public class Lesson
    {
        public string CourseName { get; set; }

        public DateTime StartTime { get; set; }

        public string StartedBy { get; set; }

        public string RoomName { get; set; }

        public string SeatingPlanId { get; set; }

        public string DisplayName { get; set; }

        public DateTime LastTimeExtension { get; set; }

        public string Subject { get; set; }

        public ICollection<string> Participants { get; set; }

        public Enums.LessonPhase CurrentPhase { get; set; }
    }
}
