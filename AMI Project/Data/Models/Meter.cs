using System;
using System.Collections.Generic;

namespace AMI_Project.Data.Models;

public partial class Meter
{
    public string MeterSerialNo { get; set; } = null!;

    public string IpAddress { get; set; } = null!;

    public string ICCID { get; set; } = null!;

    public string IMSI { get; set; } = null!;

    public string Manufacturer { get; set; } = null!;

    public string? Firmware { get; set; }

    public string Category { get; set; } = null!;

    public DateTime InstallTsUtc { get; set; }

    public string Status { get; set; } = null!;

    public long? ConsumerId { get; set; }

    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();

    public virtual Consumer? Consumer { get; set; }

    public virtual ICollection<MeterReading> MeterReadings { get; set; } = new List<MeterReading>();

    public virtual ICollection<MonthlyMeterReading> MonthlyMeterReadings { get; set; } = new List<MonthlyMeterReading>();
}
