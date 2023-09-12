using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeSchedule.Models
{
    public class TeachersSchedule
    {
        public int Id { get; set; }
        public int DayOfTheWeek { get; set; }
        public int Shift { get; set; }
        public int SubjectNumber { get; set; }
        [ForeignKey("TeacherId")]
        public virtual Teacher Teacher { get; set; }
        public int? TeacherId { get; set; }
        [ForeignKey("ScheduleDenominatorId")]
        public virtual Schedule ScheduleDenominator { get; set; }
        public int? ScheduleDenominatorId { get; set; }
        [ForeignKey("ScheduleNumeratorId")]
        public virtual Schedule ScheduleNumerator { get; set; }
        public int? ScheduleNumeratorId { get; set; }
    }
}
