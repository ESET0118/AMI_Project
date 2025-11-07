using System.ComponentModel.DataAnnotations;

namespace AMI_Project.DTOs.Tariffs
{
    public class TariffCreateDto
    {
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public DateOnly EffectiveFrom { get; set; }

        public DateOnly? EffectiveTo { get; set; }

        [Range(0, double.MaxValue)]
        public decimal BaseRate { get; set; }

        [Range(0, 100)]
        public decimal TaxRate { get; set; }
    }
}
