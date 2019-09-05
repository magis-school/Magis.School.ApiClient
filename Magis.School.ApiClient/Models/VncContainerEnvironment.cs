using System.Collections.Generic;

namespace Magis.School.ApiClient.Models
{
    public class VncContainerEnvironment
    {
        public VncContainer VncContainer { get; set; }

        public User OwnerUser { get; set; }

        public ICollection<Course> Courses { get; set; }

        public Enums.ActiveVncContainerView ActiveView { get; set; }
    }
}
