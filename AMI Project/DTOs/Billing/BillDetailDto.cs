
namespace AMI_Project.DTOs.Billing
{
    public class BillDetailDto
    {
        public int? TariffSlabId { get; set; }
        public decimal Units { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
    }
}
