using System.Collections.Generic;

namespace CollegeSchedule.Models
{
    public class Teacher
    {
        public  Teacher()
        {
            ExamsSchedules = new List<ExamsSchedule>();
            ConsultationsSchedule = new List<ConsultationSchedule>();
        }
        public int Id { get; set; }
        public string teacherFullName { get; set; }
        public string TeacherPassword { get; set; }
        public List<ExamsSchedule> ExamsSchedules { get; set; }
        public List<ConsultationSchedule> ConsultationsSchedule { get; set; }
    }
}
