using System;
using System.Collections.Generic;

namespace AMI_Project.DTOs.Billing
{
    public class BillCreateDto
    {
        public long ConsumerId { get; set; }
        public string? MeterSerialNo { get; set; }
        public DateOnly BillingPeriodStart { get; set; }
        public DateOnly BillingPeriodEnd { get; set; }
        public decimal UnitsConsumed { get; set; }
        public decimal TotalAmount { get; set; }
        public int TariffId { get; set; }

        public List<BillDetailCreateDto>? BillDetails { get; set; }
    }
}
