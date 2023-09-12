using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeSchedule.Models
{
    public class ConsultationSchedule
    {
        public int Id { get; set; }
        public int ConsultationNumber { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Room { get; set; }
        [ForeignKey("ConsultationSubjectId")]
        public virtual Subject ConsultationSubject { get; set; }
        public int? ConsultationSubjectId { get; set; }
        [ForeignKey("TeacherId")]
        public virtual Teacher Teacher { get; set; }
        public int? TeacherId { get; set; }
        [ForeignKey("GroupId")]
        public virtual Group Group { get; set; }
        public int? GroupId { get; set; }
    }
}
