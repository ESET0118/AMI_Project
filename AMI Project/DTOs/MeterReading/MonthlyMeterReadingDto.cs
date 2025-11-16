namespace AMI_Project.DTOs.MeterReadings
{
    public class MonthlyMeterReadingDto
    {
        public long MonthlyMeterReadingId { get; set; }
        public string MeterSerialNo { get; set; } = null!;
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal TotalConsumptionKwh { get; set; }
    }
}
