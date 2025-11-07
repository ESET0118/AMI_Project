namespace AMI_Project.DTOs.Billing
{
    public class BillDetailReadDto
    {
        public long BillDetailId { get; set; }
        public decimal Units { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public int? TariffSlabId { get; set; }
    }
}
