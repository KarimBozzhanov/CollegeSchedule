using Microsoft.Net.Http.Headers;

namespace CollegeSchedule.Models
{
    public class PracticeSchedule
    {
        public int Id { get; set; }
        public int Cource { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}
