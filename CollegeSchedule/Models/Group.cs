using System.Collections.Generic;

namespace CollegeSchedule.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
        public int GroupCourse { get; set; }
        public int GroupShift { get; set; }
    }
}
