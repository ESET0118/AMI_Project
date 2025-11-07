namespace AMI_Project.DTOs.Auth
{
    public class RegisterDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? DisplayName { get; set; }
        public string? Phone { get; set; }
        public string? Role { get; set; }  // 🆕 Added for role-based registration ("Admin" / "User")
    }
}
