using System;
using System.Collections.Generic;

namespace AMI_Project.Data.Models;

public partial class BillDetail
{
    public long BillDetailId { get; set; }

    public long BillId { get; set; }

    public int? TariffSlabId { get; set; }

    public decimal Units { get; set; }

    public decimal Rate { get; set; }

    public decimal Amount { get; set; }

    public virtual Bill Bill { get; set; } = null!;

    public virtual TariffSlab? TariffSlab { get; set; }
}
