using AMI_Project.Data.Models;
using AMI_Project.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AMI_Project.Repositories.Interfaces
{
    public interface IMeterReadingRepository
    {
        Task<IEnumerable<MeterReading>> GetAllAsync(CancellationToken ct);
        Task<MeterReading?> GetByIdAsync(long id, CancellationToken ct);
        Task<IEnumerable<MeterReading>> GetByMeterSerialNoAsync(string serialNo, CancellationToken ct);
        // New: get readings for a specific year/month (date range)
        Task<IEnumerable<MeterReading>> GetByMeterSerialNoForMonthAsync(string serialNo, int year, int month, CancellationToken ct);
        Task<MeterReading> AddAsync(MeterReading entity, CancellationToken ct);
        Task<IEnumerable<MeterReading>> AddRangeAsync(IEnumerable<MeterReading> entities, CancellationToken ct);
        Task<MeterReading> UpdateAsync(MeterReading entity, CancellationToken ct);
        Task<IEnumerable<MeterReading>> UpdateRangeAsync(IEnumerable<MeterReading> entities, CancellationToken ct);
        Task DeleteAsync(long id, CancellationToken ct);

        // Monthly entity operations
        Task<MonthlyMeterReading?> GetMonthlyAsync(string serialNo, int year, int month, CancellationToken ct);
        Task<MonthlyMeterReading> UpsertMonthlyAsync(MonthlyMeterReading entity, CancellationToken ct);
    }
}
