using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AMI_Project.Models;

[Table("Tariff")]
public partial class Tariff
{
    [Key]
    public int TariffId { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    public DateOnly EffectiveFrom { get; set; }

    public DateOnly? EffectiveTo { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal BaseRate { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal TaxRate { get; set; }

    [Precision(3)]
    public DateTime CreatedAt { get; set; }

    [InverseProperty("Tariff")]
    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();

    [InverseProperty("Tariff")]
    public virtual ICollection<Consumer> Consumers { get; set; } = new List<Consumer>();

    [InverseProperty("Tariff")]
    public virtual ICollection<TariffSlab> TariffSlabs { get; set; } = new List<TariffSlab>();
}
