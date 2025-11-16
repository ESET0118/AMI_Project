using System;

namespace AMI_Project.DTOs.MeterReadings
{
    public class DailyMeterReadingDto
    {
        public DateTime Date { get; set; }             // Date component only (UTC)
        public long? MeterReadingId { get; set; }      // null if no reading exists
        public decimal? ConsumptionKwh { get; set; }   // null if missing
        public decimal? Voltage { get; set; }
        public decimal? Ampere { get; set; }
        public decimal? PowerFactor { get; set; }
        public decimal? Frequency { get; set; }
    }
}
