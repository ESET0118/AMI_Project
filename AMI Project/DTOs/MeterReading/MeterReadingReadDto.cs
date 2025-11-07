using System;

namespace AMI_Project.DTOs.MeterReadings
{
    public class MeterReadingReadDto
    {
        public long MeterReadingId { get; set; }
        public string MeterSerialNo { get; set; } = null!;
        public DateTime ReadingDateTime { get; set; }
        public decimal ConsumptionKwh { get; set; }
        public decimal? Voltage { get; set; }
        public decimal? Ampere { get; set; }
        public decimal? PowerFactor { get; set; }
        public decimal? Frequency { get; set; }
    }
}
