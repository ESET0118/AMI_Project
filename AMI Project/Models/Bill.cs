using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AMI_Project.Models;

[Table("Bill")]
public partial class Bill
{
    [Key]
    public long BillId { get; set; }

    public long ConsumerId { get; set; }

    [StringLength(50)]
    public string? MeterSerialNo { get; set; }

    public DateOnly BillingPeriodStart { get; set; }

    public DateOnly BillingPeriodEnd { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal UnitsConsumed { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal TotalAmount { get; set; }

    public int TariffId { get; set; }

    [Precision(3)]
    public DateTime BillGeneratedAt { get; set; }

    [InverseProperty("Bill")]
    public virtual ICollection<BillDetail> BillDetails { get; set; } = new List<BillDetail>();

    [ForeignKey("ConsumerId")]
    [InverseProperty("Bills")]
    public virtual Consumer Consumer { get; set; } = null!;

    [ForeignKey("MeterSerialNo")]
    [InverseProperty("Bills")]
    public virtual Meter? MeterSerialNoNavigation { get; set; }

    [ForeignKey("TariffId")]
    [InverseProperty("Bills")]
    public virtual Tariff Tariff { get; set; } = null!;
}
