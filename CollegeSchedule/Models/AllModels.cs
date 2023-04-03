using System.Collections;
using System.Collections.Generic;

namespace CollegeSchedule.Models
{
    public class AllModels
    {
        public IEnumerable<Schedule> Schedules { get; set; }
        public IEnumerable<Teacher> Teachers { get; set; }
    }
}
