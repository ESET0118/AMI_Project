using AMI_Project.Models;

namespace AMI_Project.Repositories.Interfaces
{
    public interface IMeterReadingRepository
    {
        Task<IEnumerable<MeterReading>> GetAllAsync(CancellationToken ct);
        Task<MeterReading?> GetByIdAsync(long id, CancellationToken ct);
        Task<IEnumerable<MeterReading>> GetByMeterSerialNoAsync(string serialNo, CancellationToken ct);
        Task<MeterReading> AddAsync(MeterReading entity, CancellationToken ct);
        Task<MeterReading> UpdateAsync(MeterReading entity, CancellationToken ct);
        Task DeleteAsync(long id, CancellationToken ct);
    }
}
