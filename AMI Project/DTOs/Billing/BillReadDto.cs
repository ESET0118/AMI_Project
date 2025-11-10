namespace AMI_Project.DTOs.Billing
{
    public class BillReadDto
    {
        public long BillId { get; set; } // Optional if not saved to DB
        public string MeterId { get; set; }
        public long MeterReadingId { get; set; }
        public decimal BaseAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public List<BillDetailReadDto> BillDetails { get; set; } = new();
    }
}
