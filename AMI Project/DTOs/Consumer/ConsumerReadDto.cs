namespace AMI_Project.DTOs.Consumers
{
    public class ConsumerReadDto
    {
        public long ConsumerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal? Lat { get; set; }
        public decimal? Lon { get; set; }
        public DateTime CreatedAt { get; set; }

        // Added fields used by the UI / service
        public int MeterCount { get; set; }                 // number of meters for this consumer
        public string? OrgUnitName { get; set; }            // optional name of org unit
        public string? TariffName { get; set; }             // optional name of tariff
    }
}
