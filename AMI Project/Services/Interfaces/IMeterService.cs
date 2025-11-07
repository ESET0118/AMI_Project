using AMI_Project.DTOs.Meter;
using AMI_Project.Helpers;
using Microsoft.AspNetCore.Http;


namespace AMI_Project.Services.Interfaces
{
    public interface IMeterService
    {
        Task<PagedResult<MeterReadDto>> GetMetersAsync(MeterFilterDto filter, CancellationToken ct);
        Task<MeterReadDto?> GetBySerialAsync(string serialNo, CancellationToken ct);
        Task<MeterReadDto> CreateAsync(MeterCreateDto dto, CancellationToken ct);
        Task<MeterReadDto> UpdateAsync(string serialNo, MeterUpdateDto dto, CancellationToken ct);
        Task DeleteAsync(string serialNo, CancellationToken ct);

        // ✅ NEW
        Task<object> UploadCsvAsync(IFormFile file, CancellationToken ct);
    }
}
