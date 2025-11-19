using System;
using System.Collections.Generic;

namespace AMI_Project.Data.Models;

public partial class TariffSlab
{
    public int TariffSlabId { get; set; }

    public int TariffId { get; set; }

    public decimal FromKwh { get; set; }

    public decimal ToKwh { get; set; }

    public decimal RatePerKwh { get; set; }

    public int? Sequence { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<BillDetail> BillDetails { get; set; } = new List<BillDetail>();

    public virtual Tariff Tariff { get; set; } = null!;
}
