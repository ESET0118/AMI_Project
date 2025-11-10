using AMI_Project.DTOs.Meters;
using AMI_Project.Models;
using System.Threading;

namespace AMI_Project.Services.Interfaces
{
    public interface IMeterCsvService
    {
        Task<MeterCsvUploadResult> UploadAndImportAsync(MeterUploadResultDto dto, CancellationToken ct);
    }

    public class MeterCsvUploadResult
    {
        public IEnumerable<Meter> ImportedMeters { get; set; } = new List<Meter>();
        public IEnumerable<string> Warnings { get; set; } = new List<string>();
    }
}