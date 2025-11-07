using AMI_Project.Models;

namespace AMI_Project.Repositories.Interfaces
{
    public interface IMeterReadingRepository
    {
        Task<IEnumerable<MeterReading>> GetAllAsync(CancellationToken ct);
        Task<MeterReading?> GetByIdAsync(long id, CancellationToken ct);
        Task<IEnumerable<MeterReading>> GetByMeterSerialNoAsync(string meterSerialNo, CancellationToken ct);
        Task<MeterReading> AddAsync(MeterReading reading, CancellationToken ct);
        Task<MeterReading?> UpdateAsync(MeterReading reading, CancellationToken ct);
        Task DeleteAsync(long id, CancellationToken ct);
    }
}
