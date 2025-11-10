using AMI_Project.Data;
using AMI_Project.Models;
using AMI_Project.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AMI_Project.Repositories
{
    public class BillRepository : IBillRepository
    {
        private readonly AMIDbContext _context;

        public BillRepository(AMIDbContext context)
        {
            _context = context;
        }

        public async Task<Bill> CreateAsync(Bill bill)
        {
            _context.Bills.Add(bill);
            await _context.SaveChangesAsync();
            return bill;
        }

        public async Task<IEnumerable<Bill>> GetAllAsync()
        {
            return await _context.Bills.ToListAsync();
        }

        public async Task<Bill?> GetByIdAsync(long id)
        {
            return await _context.Bills.FindAsync(id);
        }

        // ✅ Fixed: Get Tariff via Consumer
        public async Task<Tariff?> GetTariffByConsumerIdAsync(long consumerId)
        {
            // 1️⃣ Get the Consumer
            var consumer = await _context.Consumers
                .FirstOrDefaultAsync(c => c.ConsumerId == consumerId);

            if (consumer == null)
                return null;

            // 2️⃣ Get the Tariff for this consumer
            var tariff = await _context.Tariffs
                .Include(t => t.TariffSlabs)
                .FirstOrDefaultAsync(t => t.TariffId == consumer.TariffId);

            return tariff;
        }
    }
}
