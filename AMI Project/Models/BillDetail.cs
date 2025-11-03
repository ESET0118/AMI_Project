using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AMI_Project.Models;

[Table("BillDetail")]
public partial class BillDetail
{
    [Key]
    public long BillDetailId { get; set; }

    public long BillId { get; set; }

    public int? TariffSlabId { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal Units { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal Rate { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal Amount { get; set; }

    [ForeignKey("BillId")]
    [InverseProperty("BillDetails")]
    public virtual Bill Bill { get; set; } = null!;

    [ForeignKey("TariffSlabId")]
    [InverseProperty("BillDetails")]
    public virtual TariffSlab? TariffSlab { get; set; }
}
