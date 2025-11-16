using AMI_Project.Models;
using System;
using System.Collections.Generic;

namespace AMI_Project.Data.Models;

public partial class MonthlyMeterReading
{
    public long MonthlyMeterReadingId { get; set; }

    public string MeterSerialNo { get; set; } = null!;

    public int Year { get; set; }

    public int Month { get; set; }

    public decimal TotalConsumptionKwh { get; set; }

    public virtual Meter MeterSerialNoNavigation { get; set; } = null!;
}
