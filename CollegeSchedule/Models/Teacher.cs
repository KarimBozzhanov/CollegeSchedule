using System.Collections.Generic;

namespace CollegeSchedule.Models
{
    public class Teacher
    {
        public  Teacher()
        {
            ExamsSchedules = new List<ExamsSchedule>();
        }
        public int Id { get; set; }
        public string teacherFullName { get; set; }
        public List<ExamsSchedule> ExamsSchedules { get; set; }
    }
}
