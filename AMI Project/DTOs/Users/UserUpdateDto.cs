using System.ComponentModel.DataAnnotations;

namespace AMI_Project.DTOs.Users
{
    public class UserUpdateDto
    {
        [StringLength(200)]
        public string? DisplayName { get; set; }

        [Phone]
        public string? Phone { get; set; }

        public bool? EmailConfirmed { get; set; }

        public string? Role { get; set; }
    }
}
