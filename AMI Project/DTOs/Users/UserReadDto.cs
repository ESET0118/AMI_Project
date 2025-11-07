namespace AMI_Project.DTOs.Users
{
    public class UserReadDto
    {
        public long UserId { get; set; }
        public string Email { get; set; } = null!;
        public string? DisplayName { get; set; }
        public string? Phone { get; set; }
        public bool EmailConfirmed { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Role { get; set; } // Admin or User
    }
}
