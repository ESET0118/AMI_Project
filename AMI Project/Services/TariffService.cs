using AMI_Project.Data;
using AMI_Project.Models;
using AMI_Project.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AMI_Project.Services.Implementations
{
    public class TariffService : ITariffService
    {
        private readonly AMIDbContext _context;

        public TariffService(AMIDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Tariff>> GetAllAsync(CancellationToken ct)
        {
            return await _context.Tariffs.AsNoTracking().ToListAsync(ct);
        }

        public async Task<Tariff?> GetByIdAsync(int id, CancellationToken ct)
        {
            return await _context.Tariffs.FindAsync(new object[] { id }, ct);
        }

        public async Task<Tariff> CreateAsync(Tariff tariff, CancellationToken ct)
        {
            _context.Tariffs.Add(tariff);
            await _context.SaveChangesAsync(ct);
            return tariff;
        }

        public async Task<Tariff?> UpdateAsync(int id, Tariff tariff, CancellationToken ct)
        {
            var existing = await _context.Tariffs.FindAsync(new object[] { id }, ct);
            if (existing == null) return null;

            existing.Name = tariff.Name ?? existing.Name;
            existing.EffectiveFrom = tariff.EffectiveFrom != default ? tariff.EffectiveFrom : existing.EffectiveFrom;
            existing.EffectiveTo = tariff.EffectiveTo ?? existing.EffectiveTo;
            existing.BaseRate = tariff.BaseRate != default ? tariff.BaseRate : existing.BaseRate;
            existing.TaxRate = tariff.TaxRate != default ? tariff.TaxRate : existing.TaxRate;

            await _context.SaveChangesAsync(ct);
            return existing;
        }

        public async Task DeleteAsync(int id, CancellationToken ct)
        {
            var entity = await _context.Tariffs.FindAsync(new object[] { id }, ct);
            if (entity != null)
            {
                _context.Tariffs.Remove(entity);
                await _context.SaveChangesAsync(ct);
            }
        }
    }
}
