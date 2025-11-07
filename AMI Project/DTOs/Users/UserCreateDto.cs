using System.ComponentModel.DataAnnotations;

namespace AMI_Project.DTOs.Users
{
    public class UserCreateDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = null!;

        public string? DisplayName { get; set; }

        [Phone]
        public string? Phone { get; set; }

        public string Role { get; set; } = "User"; // Default role
    }
}
