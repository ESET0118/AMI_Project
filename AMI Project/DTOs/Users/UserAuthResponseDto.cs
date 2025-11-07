namespace AMI_Project.DTOs.Users
{
    public class UserAuthResponseDto
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public long UserId { get; set; }
        public string Email { get; set; } = null!;
        public string? DisplayName { get; set; }
        public string? Role { get; set; }
    }
}
