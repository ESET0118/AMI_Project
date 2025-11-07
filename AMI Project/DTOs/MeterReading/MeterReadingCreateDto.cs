using System;
using System.ComponentModel.DataAnnotations;

namespace AMI_Project.DTOs.MeterReadings
{
    public class MeterReadingCreateDto
    {
        [Required, StringLength(50)]
        public string MeterSerialNo { get; set; } = null!;

        [Required]
        public DateTime ReadingDateTime { get; set; } = DateTime.UtcNow;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal ConsumptionKwh { get; set; }

        public decimal? Voltage { get; set; }
        public decimal? Ampere { get; set; }
        public decimal? PowerFactor { get; set; }
        public decimal? Frequency { get; set; }
    }
}
