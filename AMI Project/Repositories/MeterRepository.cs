using AMI_Project.Data;
using AMI_Project.Models;
using AMI_Project.Helpers;
using Microsoft.EntityFrameworkCore;

namespace AMI_Project.Repositories
{
    public class MeterRepository : IMeterRepository
    {
        private readonly AMIDbContext _context;

        public MeterRepository(AMIDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Meter>> GetMetersAsync(
            string? serialNo,
            string? status,
            long? consumerId,
            DateTime? fromInstall,
            DateTime? toInstall,
            int page,
            int pageSize,
            CancellationToken ct)
        {
            var query = _context.Meters
                .AsNoTracking()
                .Include(m => m.Consumer)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(serialNo))
                query = query.Where(m => m.MeterSerialNo.Contains(serialNo));
            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(m => m.Status == status);
            if (consumerId.HasValue)
                query = query.Where(m => m.ConsumerId == consumerId);
            if (fromInstall.HasValue)
                query = query.Where(m => m.InstallTsUtc >= fromInstall);
            if (toInstall.HasValue)
                query = query.Where(m => m.InstallTsUtc <= toInstall);

            var total = await query.CountAsync(ct);
            var items = await query
                .OrderBy(m => m.MeterSerialNo)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PagedResult<Meter>
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public Task<Meter?> GetBySerialAsync(string serialNo, CancellationToken ct) =>
            _context.Meters.Include(m => m.Consumer)
                .FirstOrDefaultAsync(m => m.MeterSerialNo == serialNo, ct);

        public async Task AddAsync(Meter meter, CancellationToken ct)
        {
            await _context.Meters.AddAsync(meter, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Meter meter, CancellationToken ct)
        {
            _context.Meters.Update(meter);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(string serialNo, CancellationToken ct)
        {
            var entity = await _context.Meters.FindAsync(new object[] { serialNo }, ct);
            if (entity != null)
            {
                _context.Meters.Remove(entity);
                await _context.SaveChangesAsync(ct);
            }
        }

        public async Task AddOrUpdateBulkAsync(IEnumerable<Meter> meters, CancellationToken ct)
        {
            foreach (var meter in meters)
            {
                var existing = await _context.Meters
                    .FirstOrDefaultAsync(m => m.MeterSerialNo == meter.MeterSerialNo, ct);

                if (existing == null)
                    _context.Meters.Add(meter);
                else
                {
                    _context.Entry(existing).CurrentValues.SetValues(meter);
                }
            }
            await _context.SaveChangesAsync(ct);
        }

        public Task<bool> ConsumerExistsAsync(long consumerId, CancellationToken ct) =>
            _context.Consumers.AnyAsync(c => c.ConsumerId == consumerId, ct);
    }
}
