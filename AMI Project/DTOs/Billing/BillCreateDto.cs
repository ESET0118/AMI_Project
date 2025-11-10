using System;
using System.Collections.Generic;

namespace AMI_Project.DTOs.Billing
{
    public class BillCreateDto
    {
        public long MeterReadingId { get; set; }
        public int MeterId { get; set; }
    }
}
