using AMI_Project.DTOs.MeterReadings;

namespace AMI_Project.Services.Interfaces
{
    public interface IMeterReadingService
    {
        Task<IEnumerable<MeterReadingReadDto>> GetAllAsync(CancellationToken ct);
        Task<MeterReadingReadDto?> GetByIdAsync(long id, CancellationToken ct);
        Task<IEnumerable<MeterReadingReadDto>> GetByMeterSerialNoAsync(string meterSerialNo, CancellationToken ct);
        Task<MeterReadingReadDto> CreateAsync(MeterReadingCreateDto dto, CancellationToken ct);
        Task<MeterReadingReadDto?> UpdateAsync(long id, MeterReadingUpdateDto dto, CancellationToken ct);
        Task DeleteAsync(long id, CancellationToken ct);
    }
}
