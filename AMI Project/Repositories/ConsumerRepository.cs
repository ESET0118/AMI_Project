using AMI_Project.Data;
using AMI_Project.Models;
using AMI_Project.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AMI_Project.Repositories
{
    public class ConsumerRepository : IConsumerRepository
    {
        private readonly AMIDbContext _context;

        public ConsumerRepository(AMIDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Consumer>> GetAllAsync(CancellationToken ct)
        {
            return await _context.Consumers
                .AsNoTracking()
                .Include(c => c.OrgUnit)
                .Include(c => c.Tariff)
                .ToListAsync(ct);
        }

        public async Task<Consumer?> GetByIdAsync(long id, CancellationToken ct)
        {
            return await _context.Consumers
                .Include(c => c.OrgUnit)
                .Include(c => c.Tariff)
                .FirstOrDefaultAsync(c => c.ConsumerId == id, ct);
        }

        public async Task<Consumer> AddAsync(Consumer consumer, CancellationToken ct)
        {
            await _context.Consumers.AddAsync(consumer, ct);
            await _context.SaveChangesAsync(ct);
            return consumer;
        }

        public async Task<Consumer?> UpdateAsync(Consumer consumer, CancellationToken ct)
        {
            var existing = await _context.Consumers
                .FirstOrDefaultAsync(c => c.ConsumerId == consumer.ConsumerId, ct);

            if (existing == null)
                return null;

            // ✅ Manually update only modified fields (for safety)
            _context.Entry(existing).CurrentValues.SetValues(consumer);
            _context.Entry(existing).State = EntityState.Modified;

            await _context.SaveChangesAsync(ct);
            return existing;
        }

        public async Task DeleteAsync(long id, CancellationToken ct)
        {
            var consumer = await _context.Consumers.FindAsync(new object[] { id }, ct);
            if (consumer == null) return;

            _context.Consumers.Remove(consumer);
            await _context.SaveChangesAsync(ct);
        }
    }
}
