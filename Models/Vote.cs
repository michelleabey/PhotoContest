namespace PhotoContest.Models
{
    public class Vote
    {
        public int Id { get; set; }
        public int PhotoId { get; set; }
        public int UserId { get; set; }
        public DateTime VoteDate { get; set; }
    }
}
