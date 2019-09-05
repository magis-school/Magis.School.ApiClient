namespace Magis.School.ApiClient.Models
{
    public class ComputerSessionStatus
    {
        public User User { get; set; }

        public Enums.ActiveComputerSessionView ActiveView { get; set; }

        public Course Course { get; set; }

        public Lesson Lesson { get; set; }
    }
}
