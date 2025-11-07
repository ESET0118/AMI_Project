using System;
using System.Collections.Generic;

namespace AMI_Project.DTOs.Billing
{
    public class BillReadDto
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

        public List<BillDetailReadDto>? BillDetails { get; set; }
    }
}
