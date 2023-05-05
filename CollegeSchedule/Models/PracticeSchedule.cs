using Microsoft.Net.Http.Headers;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeSchedule.Models
{
    public class PracticeSchedule
    {
        public int Id { get; set; }
        public int PracticeNumber { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string PracticeName { get; set; }
        [ForeignKey("TeacherId")]
        public virtual Teacher Teacher { get; set; }
        public int? TeacherId { get; set; }
        [ForeignKey("GroupId")]
        public virtual Group Group { get; set; }
        public int? GroupId { get; set; }
    }
}
