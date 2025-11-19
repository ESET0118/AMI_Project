using System;
using System.Collections.Generic;

namespace AMI_Project.Data.Models;

public partial class Tariff
{
    public int TariffId { get; set; }

    public string Name { get; set; } = null!;

    public DateOnly EffectiveFrom { get; set; }

    public DateOnly? EffectiveTo { get; set; }

    public decimal BaseRate { get; set; }

    public decimal TaxRate { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();

    public virtual ICollection<Consumer> Consumers { get; set; } = new List<Consumer>();

    public virtual ICollection<TariffSlab> TariffSlabs { get; set; } = new List<TariffSlab>();
}
