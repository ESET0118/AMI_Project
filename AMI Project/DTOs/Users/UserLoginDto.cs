using System.ComponentModel.DataAnnotations;

namespace AMI_Project.DTOs.Users
{
    public class UserLoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}
