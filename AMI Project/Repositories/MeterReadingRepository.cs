using AMI_Project.Data;
using AMI_Project.Data.Models;
using AMI_Project.Models;
using AMI_Project.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AMI_Project.Repositories
{
    public class MeterReadingRepository : IMeterReadingRepository
    {
        private readonly AMIDbContext _context;

        public MeterReadingRepository(AMIDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MeterReading>> GetAllAsync(CancellationToken ct)
        {
            return await _context.MeterReadings
                .AsNoTracking()
                .OrderByDescending(r => r.ReadingDateTime)
                .ToListAsync(ct);
        }

        public async Task<MeterReading?> GetByIdAsync(long id, CancellationToken ct)
        {
            return await _context.MeterReadings
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.MeterReadingId == id, ct);
        }

        public async Task<IEnumerable<MeterReading>> GetByMeterSerialNoAsync(string serialNo, CancellationToken ct)
        {
            return await _context.MeterReadings
                .AsNoTracking()
                .Where(r => r.MeterSerialNo == serialNo)
                .OrderByDescending(r => r.ReadingDateTime)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<MeterReading>> GetByMeterSerialNoForMonthAsync(string serialNo, int year, int month, CancellationToken ct)
        {
            var start = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
            var end = start.AddMonths(1);
            return await _context.MeterReadings
                .AsNoTracking()
                .Where(r => r.MeterSerialNo == serialNo && r.ReadingDateTime >= start && r.ReadingDateTime < end)
                .OrderBy(r => r.ReadingDateTime)
                .ToListAsync(ct);
        }

        public async Task<MeterReading> AddAsync(MeterReading entity, CancellationToken ct)
        {
            await _context.MeterReadings.AddAsync(entity, ct);
            await _context.SaveChangesAsync(ct);
            return entity;
        }

        public async Task<IEnumerable<MeterReading>> AddRangeAsync(IEnumerable<MeterReading> entities, CancellationToken ct)
        {
            await _context.MeterReadings.AddRangeAsync(entities, ct);
            await _context.SaveChangesAsync(ct);
            return entities.ToList();
        }

        public async Task<MeterReading> UpdateAsync(MeterReading entity, CancellationToken ct)
        {
            _context.MeterReadings.Update(entity);
            await _context.SaveChangesAsync(ct);
            return entity;
        }

        public async Task<IEnumerable<MeterReading>> UpdateRangeAsync(IEnumerable<MeterReading> entities, CancellationToken ct)
        {
            _context.MeterReadings.UpdateRange(entities);
            await _context.SaveChangesAsync(ct);
            return entities.ToList();
        }

        public async Task DeleteAsync(long id, CancellationToken ct)
        {
            var entity = await _context.MeterReadings.FindAsync(new object[] { id }, ct);
            if (entity != null)
            {
                _context.MeterReadings.Remove(entity);
                await _context.SaveChangesAsync(ct);
            }
        }

        // Monthly entity operations
        public async Task<MonthlyMeterReading?> GetMonthlyAsync(string serialNo, int year, int month, CancellationToken ct)
        {
            return await _context.MonthlyMeterReadings
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.MeterSerialNo == serialNo && m.Year == year && m.Month == month, ct);
        }

        public async Task<MonthlyMeterReading> UpsertMonthlyAsync(MonthlyMeterReading entity, CancellationToken ct)
        {
            var existing = await _context.MonthlyMeterReadings
                .FirstOrDefaultAsync(m => m.MeterSerialNo == entity.MeterSerialNo && m.Year == entity.Year && m.Month == entity.Month, ct);

            if (existing == null)
            {
                await _context.MonthlyMeterReadings.AddAsync(entity, ct);
            }
            else
            {
                existing.TotalConsumptionKwh = entity.TotalConsumptionKwh;
                _context.MonthlyMeterReadings.Update(existing);
                entity.MonthlyMeterReadingId = existing.MonthlyMeterReadingId;
            }

            await _context.SaveChangesAsync(ct);
            return entity;
        }
    }
}
