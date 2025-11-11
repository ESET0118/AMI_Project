using System.ComponentModel.DataAnnotations;



public class MeterCreateDto
{
    [Required]
    public string MeterSerialNo { get; set; } = string.Empty;

    [Required]
    public string IpAddress { get; set; } = string.Empty;

    [Required]
    public string Iccid { get; set; } = string.Empty;

    [Required]
    public string Imsi { get; set; } = string.Empty;

    [Required]
    public string Manufacturer { get; set; } = string.Empty;

    public string? Firmware { get; set; }
    public string Category { get; set; } = string.Empty;
    public long? ConsumerId { get; set; }

    public string ConsumerName { get; set; } = string.Empty; // Changed from ConsumerId

}

