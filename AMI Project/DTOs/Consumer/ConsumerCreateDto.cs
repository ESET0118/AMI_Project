using System.ComponentModel.DataAnnotations;

namespace AMI_Project.DTOs.Consumers
{
    public class ConsumerCreateDto
    {
        [Required, StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Address { get; set; }

        [Phone]
        [StringLength(30)]
        public string? Phone { get; set; }

        [EmailAddress]
        [StringLength(200)]
        public string? Email { get; set; }

        [Required]
        public int OrgUnitId { get; set; }

        [Required]
        public int TariffId { get; set; }

        public decimal? Lat { get; set; }
        public decimal? Lon { get; set; }
    }
}
