namespace PhotoContest.DTOs
{
    public class UserRegisterDto
    {
        public required string Username { get; set; }
        public string? Email { get; set; }
        public required string Password { get; set; }
    }

    public class UserLoginDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
    public class PhotoUploadDto
    {
        public string? Title { get; set; }
        public required IFormFile Image { get; set; }
    }
}

