using System;
using System.Collections.Generic;

namespace Magis.School.ApiClient.Models
{
    public class Course
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Mail { get; set; }

        public DateTime CreationTime { get; set; }

        public string DN { get; set; }

        public string SID { get; set; }

        public uint GID { get; set; }

        public ICollection<string> Members { get; set; }

        public bool LessonStarted { get; set; }
    }
}
