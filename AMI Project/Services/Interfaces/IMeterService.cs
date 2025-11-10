using AMI_Project.DTOs.Meter;
using Microsoft.AspNetCore.Http;
using AMI_Project.Helpers;

namespace AMI_Project.Services.Interfaces
{
    public interface IMeterService
    {
        Task<PagedResult<MeterReadDto>> GetMetersAsync(MeterFilterDto filter, CancellationToken ct);
        Task<MeterReadDto?> GetBySerialAsync(string serialNo, CancellationToken ct);
        Task<MeterReadDto> CreateAsync(MeterCreateDto dto, CancellationToken ct);
        Task<MeterReadDto> UpdateAsync(string serialNo, MeterUpdateDto dto, CancellationToken ct);
        Task<PagedResult<MeterReadDto>> GetAllMetersAsync(CancellationToken ct);

        Task DeleteAsync(string serialNo, CancellationToken ct);

    }
}
