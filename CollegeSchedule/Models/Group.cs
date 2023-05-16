using System.Collections.Generic;

namespace CollegeSchedule.Models
{
    public class Group
    {
        public Group()
        {
            Schedules = new List<Schedule>();
            ExamsSchedules = new List<ExamsSchedule>();
            ConsultationsSchedule = new List<ConsultationSchedule>();
        }
        public int Id { get; set; }
        public string GroupName { get; set; }
        public int GroupCource { get; set; }
        public List<Schedule> Schedules { get; set; }
        public List<ExamsSchedule> ExamsSchedules { get; set; }
        public List<ConsultationSchedule> ConsultationsSchedule { get; set; }
    }
}
