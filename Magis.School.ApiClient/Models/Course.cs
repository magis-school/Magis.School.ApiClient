using System;

namespace Magis.School.ApiClient.Models
{
    public class Course
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public DateTime CreationTime { get; set; }

        public string DN { get; set; }

        public string SID { get; set; }

        public string[] MemberDNs { get; set; }
    }
}
