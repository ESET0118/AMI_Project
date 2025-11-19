using AMI_Project.DTOs.Billing;

namespace AMI_Project.DTOs.Billing
{

    public class BillDto
    {
        public long BillId { get; set; }
        public string MeterSerialNo { get; set; } = "";
        public decimal BillAmount { get; set; }
        public decimal TotalUnits { get; set; }
        public decimal BaseRate { get; set; }
        public decimal TaxRate { get; set; }
        public string SlabsApplied { get; set; } = "";
        public string? QrCodeUrl { get; set; }
        public bool IsPaid { get; set; }
        public decimal OutstandingAmount { get; set; }
        public long ConsumerId { get; set; }
    }
}
