using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeSchedule.Models
{
    public class Schedule
    {
        public int Id { get; set; }
        public int DayOfTheWeek { get; set; }
        public int SubjectNumber { get; set; }
        public string LessonTime { get; set; }
        public string SubjectDenominator { get; set; }
        public string SubjectNumerator { get; set; }
        public string Teacher { get; set; }
        public string Room { get; set; }
        [ForeignKey("Group")]
        public int GroupId { get; set; }
        public Group Group { get; set; }
    }
}
