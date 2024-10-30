namespace TypicalTypistAPI.Models
{
    public class UserTypingTestDTO
    {
        public int userId { get; set; }
        public DateTime? date { get; set; }
        public int? CharCount { get; set; }
        public int? incorrectCount { get; set; }
        public string? Mode { get; set; }
        public decimal? Speed { get; set; }
        public decimal? Accuracy { get; set; }
    }
}
