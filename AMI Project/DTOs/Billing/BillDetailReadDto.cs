namespace AMI_Project.DTOs.Billing
{
    public class BillDetailReadDto
    {
        public int TariffSlabId { get; set; }
        public decimal FromKwh { get; set; }
        public decimal ToKwh { get; set; }
        public decimal RatePerKwh { get; set; }
        public decimal Consumption { get; set; }
        public decimal Amount { get; set; }
    }
}
