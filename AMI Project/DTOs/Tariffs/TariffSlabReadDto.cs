namespace AMI_Project.DTOs.Tariffs
{
    public class TariffSlabReadDto
    {
        public int TariffSlabId { get; set; }
        public decimal FromKwh { get; set; }
        public decimal ToKwh { get; set; }
        public decimal RatePerKwh { get; set; }
        public int? Sequence { get; set; }
    }
}
