using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AMI_Project.DTOs.MeterReadings
{
    public class BulkMeterReadingCreateDto
    {
        [Required]
        public string MeterSerialNo { get; set; } = null!;

        [Required]
        public int Year { get; set; }

        [Required]
        [Range(1, 12)]
        public int Month { get; set; }

        [Required]
        public List<SingleDayMeterReadingDto> Readings { get; set; } = new();

        public class SingleDayMeterReadingDto
        {
            [Required]
            public DateTime Date { get; set; }   // date (UTC) - only date part used
            [Required]
            [Range(0, double.MaxValue)]
            public decimal ConsumptionKwh { get; set; }
            public decimal? Voltage { get; set; }
            public decimal? Ampere { get; set; }
            public decimal? PowerFactor { get; set; }
            public decimal? Frequency { get; set; }
            public long? MeterReadingId { get; set; } // optional: for updates
        }
    }
}
