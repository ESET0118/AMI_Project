namespace AMI_Project.DTOs.Tariffs
{
    public class TariffReadDto
    {
        public int TariffId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateOnly EffectiveFrom { get; set; }
        public DateOnly? EffectiveTo { get; set; }
        public decimal BaseRate { get; set; }
        public decimal TaxRate { get; set; }
    }
}
