using System.ComponentModel.DataAnnotations;

namespace AMI_Project.DTOs.Consumers
{
    public class ConsumerUpdateDto
    {
        [StringLength(100)]
        public string? Name { get; set; }  // ✅ Added for updating consumer name

        [StringLength(500)]
        public string? Address { get; set; }

        [Phone]
        [StringLength(30)]
        public string? Phone { get; set; }

        [EmailAddress]
        [StringLength(200)]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? Status { get; set; }

        public decimal? Lat { get; set; }
        public decimal? Lon { get; set; }

        public int? OrgUnitId { get; set; }
        public int? TariffId { get; set; }
    }
}
