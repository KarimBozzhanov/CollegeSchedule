namespace CollegeSchedule.Models
{
    public class Schedule
    {
        public int Id { get; set; }
        public string DayOfTheWeek { get; set; }
        public int SubjectNumber { get; set; }
        public string CoupleTime { get; set; }
        public string Subject { get; set; }
        public string TeacherName { get; set; }
        public int Cabinet { get; set; }
        public Group Group { get; set; }
    }
}
