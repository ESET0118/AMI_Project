using System;
using System.Collections.Generic;

namespace AMI_Project.Data.Models;

public partial class Bill
{
    public long BillId { get; set; }

    public long ConsumerId { get; set; }

    public string? MeterSerialNo { get; set; }

    public DateOnly BillingPeriodStart { get; set; }

    public DateOnly BillingPeriodEnd { get; set; }

    public decimal UnitsConsumed { get; set; }

    public decimal TotalAmount { get; set; }

    public int TariffId { get; set; }

    public DateTime BillGeneratedAt { get; set; }

    public bool IsPaid { get; set; }

    public decimal OutstandingDue { get; set; }

    public decimal TotalPayable { get; set; }

    public DateTime? PaidOn { get; set; }

    public virtual ICollection<BillDetail> BillDetails { get; set; } = new List<BillDetail>();

    public virtual Consumer Consumer { get; set; } = null!;

    public virtual Meter? MeterSerialNoNavigation { get; set; }

    public virtual Tariff Tariff { get; set; } = null!;
}
