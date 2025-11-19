using AMI_Project.Data;
using AMI_Project.Data.Models;
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
            return await _context.Bills
                .AsNoTracking()
                .OrderByDescending(b => b.BillGeneratedAt)
                .ToListAsync();
        }

        public async Task<Bill?> GetByIdAsync(long id)
        {
            return await _context.Bills
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.BillId == id);
        }

        public async Task<IEnumerable<Bill>> GetByMeterAsync(string meterSerialNo)
        {
            return await _context.Bills
                .AsNoTracking()
                .Where(b => b.MeterSerialNo == meterSerialNo)
                .OrderByDescending(b => b.BillGeneratedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Bill>> GetByConsumerAsync(long consumerId)
        {
            return await _context.Bills
                .AsNoTracking()
                .Where(b => b.ConsumerId == consumerId)
                .OrderByDescending(b => b.BillGeneratedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Bill>> GetUnpaidByConsumerAsync(long consumerId)
        {
            return await _context.Bills
                .AsNoTracking()
                .Where(b => b.ConsumerId == consumerId && !b.IsPaid)
                .OrderBy(b => b.BillingPeriodEnd)
                .ToListAsync();
        }

        public async Task<Tariff?> GetTariffByConsumerIdAsync(long consumerId)
        {
            var consumer = await _context.Consumers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ConsumerId == consumerId);

            if (consumer == null)
                return null;

            return await _context.Tariffs
                .Include(t => t.TariffSlabs)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.TariffId == consumer.TariffId);
        }

        public async Task UpdateAsync(Bill bill)
        {
            _context.Bills.Update(bill);
            await _context.SaveChangesAsync();
        }
    }
}
