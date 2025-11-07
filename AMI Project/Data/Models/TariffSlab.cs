using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace AMI_Project.Models;

[Table("TariffSlab")]
public partial class TariffSlab
{
    [Key]
    public int TariffSlabId { get; set; }

    public int TariffId { get; set; }

    [Column(TypeName = "decimal(18, 6)")]
    public decimal FromKwh { get; set; }

    [Column(TypeName = "decimal(18, 6)")]
    public decimal ToKwh { get; set; }

    [Column(TypeName = "decimal(18, 6)")]
    public decimal RatePerKwh { get; set; }

    public int? Sequence { get; set; }

    [Precision(3)]
    public DateTime CreatedAt { get; set; }

    [InverseProperty("TariffSlab")]
    public virtual ICollection<BillDetail> BillDetails { get; set; } = new List<BillDetail>();

    [ForeignKey("TariffId")]
    [InverseProperty("TariffSlabs")]
    [JsonIgnore]
    //public virtual Tariff Tariff { get; set; } = null!;

    public virtual Tariff? Tariff { get; set; }
}
