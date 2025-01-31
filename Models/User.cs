namespace PhotoContest.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string PasswordHash { get; set; } // Store hash, not plain text passwords
        public string? Email { get; set; }

    }
}
