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

        public async Task<MeterReading> AddAsync(MeterReading entity, CancellationToken ct)
        {
            await _context.MeterReadings.AddAsync(entity, ct);
            await _context.SaveChangesAsync(ct);
            return entity;
        }

        public async Task<MeterReading> UpdateAsync(MeterReading entity, CancellationToken ct)
        {
            _context.MeterReadings.Update(entity);
            await _context.SaveChangesAsync(ct);
            return entity;
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

        public async Task SaveChangesAsync(CancellationToken ct)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}
