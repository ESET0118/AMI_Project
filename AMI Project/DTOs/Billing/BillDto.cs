using AMI_Project.DTOs.Billing;

namespace AMI_Project.DTOs.Billing
{
    public class BillDto
    {
        public long BillId { get; set; }
        public long ConsumerId { get; set; }
        public string MeterSerialNo { get; set; } = null!;
        public DateOnly BillingPeriodStart { get; set; }
        public DateOnly BillingPeriodEnd { get; set; }
        public decimal UnitsConsumed { get; set; }
        public decimal TotalAmount { get; set; }
        public int TariffId { get; set; }
        public DateTime BillGeneratedAt { get; set; }
        public List<BillDetailDto> BillDetails { get; set; } = new();
    }
}
