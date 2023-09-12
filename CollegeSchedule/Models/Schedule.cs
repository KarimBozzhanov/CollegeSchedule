using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeSchedule.Models
{
    public class Schedule
    {
        public int Id { get; set; }
        public int DayOfTheWeek { get; set; }
        public int? SubjectNumber { get; set; }
        public string StartOfLesson { get; set; }
        public string EndOfLesson { get; set; }
        [ForeignKey("SubjectDenominatorId")]
        public virtual Subject SubjectDenominator { get; set; }
        public int? SubjectDenominatorId { get; set; }
        [ForeignKey("SubjectNumeratorId")]
        public virtual Subject SubjectNumerator { get; set; }
        public int? SubjectNumeratorId { get; set; }
        [ForeignKey("TeacherDenominatorId")]
        public virtual Teacher TeacherDenominator { get; set; }
        public int? TeacherDenominatorId { get; set; }
        [ForeignKey("TeacherNumeratorId")]
        public virtual Teacher TeacherNumerator { get; set; }
        public int? TeacherNumeratorId { get; set; }
        public string RoomDenominator { get; set; }
        public string RoomNumerator { get; set; }
        [ForeignKey("GroupId")]
        public virtual Group Group { get; set; }
        public int? GroupId { get; set; }
    }
}
