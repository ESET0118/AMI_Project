using AMI_Project.Data;
using AMI_Project.Models;
using AMI_Project.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AMI_Project.Repositories
{
    public class TariffRepository : ITariffRepository
    {
        private readonly AMIDbContext _context;

        public TariffRepository(AMIDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Tariff>> GetAllAsync(CancellationToken ct)
        {
            return await _context.Tariffs
                .Include(t => t.TariffSlabs)
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task<Tariff?> GetByIdAsync(int id, CancellationToken ct)
        {
            return await _context.Tariffs
                .Include(t => t.TariffSlabs)
                .FirstOrDefaultAsync(t => t.TariffId == id, ct);
        }

        public async Task<Tariff> AddAsync(Tariff tariff, CancellationToken ct)
        {
            _context.Tariffs.Add(tariff);
            await _context.SaveChangesAsync(ct);
            return tariff;
        }

        public async Task<Tariff?> UpdateAsync(Tariff tariff, CancellationToken ct)
        {
            var existing = await _context.Tariffs.FindAsync(new object[] { tariff.TariffId }, ct);
            if (existing == null) return null;

            _context.Entry(existing).CurrentValues.SetValues(tariff);
            await _context.SaveChangesAsync(ct);
            return existing;
        }

        public async Task DeleteAsync(int id, CancellationToken ct)
        {
            var tariff = await _context.Tariffs.FindAsync(new object[] { id }, ct);
            if (tariff == null) return;

            _context.Tariffs.Remove(tariff);
            await _context.SaveChangesAsync(ct);
        }
    }
}
