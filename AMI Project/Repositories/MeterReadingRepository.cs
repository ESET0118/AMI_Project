using AMI_Project.Data;
using AMI_Project.Models;
using AMI_Project.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

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
                .Include(r => r.MeterSerialNoNavigation)
                .ToListAsync(ct);
        }

        public async Task<MeterReading?> GetByIdAsync(long id, CancellationToken ct)
        {
            return await _context.MeterReadings
                .Include(r => r.MeterSerialNoNavigation)
                .FirstOrDefaultAsync(r => r.MeterReadingId == id, ct);
        }

        public async Task<IEnumerable<MeterReading>> GetByMeterSerialNoAsync(string meterSerialNo, CancellationToken ct)
        {
            return await _context.MeterReadings
                .Where(r => r.MeterSerialNo == meterSerialNo)
                .OrderByDescending(r => r.ReadingDateTime)
                .ToListAsync(ct);
        }

        public async Task<MeterReading> AddAsync(MeterReading reading, CancellationToken ct)
        {
            _context.MeterReadings.Add(reading);
            await _context.SaveChangesAsync(ct);
            return reading;
        }

        public async Task<MeterReading?> UpdateAsync(MeterReading reading, CancellationToken ct)
        {
            var existing = await _context.MeterReadings.FindAsync(new object[] { reading.MeterReadingId }, ct);
            if (existing == null) return null;

            _context.Entry(existing).CurrentValues.SetValues(reading);
            await _context.SaveChangesAsync(ct);
            return existing;
        }

        public async Task DeleteAsync(long id, CancellationToken ct)
        {
            var reading = await _context.MeterReadings.FindAsync(new object[] { id }, ct);
            if (reading == null) return;

            _context.MeterReadings.Remove(reading);
            await _context.SaveChangesAsync(ct);
        }
    }
}
