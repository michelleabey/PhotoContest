namespace PhotoContest.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime DateUploaded { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public int VoteCount { get; set; }
    
    }
}
