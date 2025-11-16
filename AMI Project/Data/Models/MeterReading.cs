using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMI_Project.Models;

[Table("MeterReading")]
public partial class MeterReading
{
    [Key]
    public long MeterReadingId { get; set; }

    [StringLength(50)]
    public string MeterSerialNo { get; set; } = null!;

    [Precision(3)]
    public DateTime ReadingDateTime { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal ConsumptionKwh { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? Voltage { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? Ampere { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal? PowerFactor { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal? Frequency { get; set; }

    [Precision(3)]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("MeterSerialNo")]
    [InverseProperty("MeterReadings")]
    public virtual Meter MeterSerialNoNavigation { get; set; } = null!;
}
