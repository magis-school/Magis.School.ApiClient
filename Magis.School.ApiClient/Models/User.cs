using System;

namespace Magis.School.ApiClient.Models
{
    public class User
    {
        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string DisplayName { get; set; }

        public string Mail { get; set; }

        public Enums.Role Role { get; set; }

        public Enums.RoleGroup[] RoleGroups { get; set; }

        public DateTime CreationTime { get; set; }

        public string DN { get; set; }

        public string ProfilePath { get; set; }

        public string SID { get; set; }

        public uint PrimaryGroupID { get; set; }

        public bool IsStudent { get; set; }

        public bool IsEmployee { get; set; }

        public bool IsTeacher { get; set; }

        public bool IsAdmin { get; set; }
    }
}
