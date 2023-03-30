using System.Collections.Generic;

namespace CollegeSchedule.Models
{
    public class Group
    {
        public Group()
        {
            Schedules = new List<Schedule>();
        }
        public int Id { get; set; }
        public string GroupName { get; set; }
        public int GroupCource { get; set; }
        public List<Schedule> Schedules { get; set; }
    }
}
