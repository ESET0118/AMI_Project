namespace AMI_Project.DTOs.Meter;


public class MeterReadDto
{
    public string MeterSerialNo { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string Iccid { get; set; } = string.Empty;
    public string Imsi { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string? Firmware { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime InstallTsUtc { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ConsumerName { get; set; }

}