using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace AMI_Project.Models;

[Table("Meter")]
public partial class Meter
{
    [Key]
    [StringLength(50)]
    public string MeterSerialNo { get; set; } = null!;

    [StringLength(45)]
    public string IpAddress { get; set; } = null!;

    [Column("ICCID")]
    [StringLength(30)]
    public string Iccid { get; set; } = null!;

    [Column("IMSI")]
    [StringLength(30)]
    public string Imsi { get; set; } = null!;

    [StringLength(100)]
    public string Manufacturer { get; set; } = null!;

    [StringLength(50)]
    public string? Firmware { get; set; }

    [StringLength(50)]
    public string Category { get; set; } = null!;

    [Precision(3)]
    public DateTime InstallTsUtc { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string Status { get; set; } = null!;

    public long? ConsumerId { get; set; }

    [InverseProperty("MeterSerialNoNavigation")]
    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();

    [ForeignKey("ConsumerId")]
    [InverseProperty("Meters")]
    public virtual Consumer? Consumer { get; set; }

    [InverseProperty("MeterSerialNoNavigation")]
    [JsonIgnore] // prevent serialization

    public virtual ICollection<MeterReading> MeterReadings { get; set; } = new List<MeterReading>();
}
