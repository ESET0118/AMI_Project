using AMI_Project.Data;
using AMI_Project.Models;
using AMI_Project.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AMI_Project.Repositories
{
    public class TariffSlabRepository : ITariffSlabRepository
    {
        private readonly AMIDbContext _context;

        public TariffSlabRepository(AMIDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TariffSlab>> GetAllAsync(CancellationToken ct)
        {
            return await _context.TariffSlabs
                .Include(s => s.Tariff)
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task<TariffSlab?> GetByIdAsync(int id, CancellationToken ct)
        {
            return await _context.TariffSlabs
                .Include(s => s.Tariff)
                .FirstOrDefaultAsync(s => s.TariffSlabId == id, ct);
        }

        public async Task<TariffSlab> AddAsync(TariffSlab slab, CancellationToken ct)
        {
            _context.TariffSlabs.Add(slab);
            await _context.SaveChangesAsync(ct);
            return slab;
        }

        public async Task<TariffSlab?> UpdateAsync(TariffSlab slab, CancellationToken ct)
        {
            var existing = await _context.TariffSlabs.FindAsync(new object[] { slab.TariffSlabId }, ct);
            if (existing == null) return null;

            _context.Entry(existing).CurrentValues.SetValues(slab);
            await _context.SaveChangesAsync(ct);
            return existing;
        }

        public async Task DeleteAsync(int id, CancellationToken ct)
        {
            var slab = await _context.TariffSlabs.FindAsync(new object[] { id }, ct);
            if (slab == null) return;

            _context.TariffSlabs.Remove(slab);
            await _context.SaveChangesAsync(ct);
        }
    }
}
