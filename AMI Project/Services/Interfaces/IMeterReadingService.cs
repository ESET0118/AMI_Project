using AMI_Project.DTOs.MeterReadings;

namespace AMI_Project.Services.Interfaces
{
    public interface IMeterReadingService
    {
        Task<IEnumerable<MeterReadingReadDto>> GetAllAsync(CancellationToken ct);
        Task<MeterReadingReadDto?> GetByIdAsync(long id, CancellationToken ct);
        Task<IEnumerable<MeterReadingReadDto>> GetByMeterSerialNoAsync(string serialNo, CancellationToken ct);
        Task<MeterReadingReadDto> CreateAsync(MeterReadingCreateDto dto, CancellationToken ct);
        Task<MeterReadingReadDto?> UpdateAsync(long id, MeterReadingUpdateDto dto, CancellationToken ct);
        Task DeleteAsync(long id, CancellationToken ct);
        Task<MeterReadingCalendarDto> GetCalendarForMonthAsync(string serialNo, int year, int month, CancellationToken ct);
        Task<IEnumerable<MeterReadingReadDto>> CreateBulkAsync(BulkMeterReadingCreateDto dto, CancellationToken ct);
        Task<MonthlyMeterReadingDto> GetMonthlyAsync(string serialNo, int year, int month, CancellationToken ct);
    }
}
