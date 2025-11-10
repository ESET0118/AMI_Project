using AMI_Project.Data;
using AMI_Project.Models;
using AMI_Project.Repositories.Interfaces;
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

        public async Task AddAsync(Meter meter, CancellationToken ct)
        {
            await _context.Meters.AddAsync(meter, ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct)
        {
            await _context.SaveChangesAsync(ct);
        }

        public async Task<Meter?> GetByIdAsync(string meterSerialNo, CancellationToken ct)
        {
            return await _context.Meters
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.MeterSerialNo == meterSerialNo, ct);
        }

        public async Task<IEnumerable<Meter>> GetAllAsync(CancellationToken ct)
        {
            return await _context.Meters
                .AsNoTracking()
                .ToListAsync(ct);
        }
    }
}
