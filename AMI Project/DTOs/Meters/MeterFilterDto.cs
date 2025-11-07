namespace AMI_Project.DTOs.Meter
{
    public class MeterFilterDto
    {
        public string? SerialNo { get; set; }
        public string? Status { get; set; }
        public long? ConsumerId { get; set; }
        public DateTime? FromInstallDate { get; set; }
        public DateTime? ToInstallDate { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
