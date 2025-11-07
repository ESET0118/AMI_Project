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

        public async Task<IEnumerable<Bill>> GetAllAsync()
        {
            return await _context.Bills
                .Include(b => b.BillDetails)
                .Include(b => b.Consumer)
                .Include(b => b.Tariff)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Bill?> GetByIdAsync(long id)
        {
            return await _context.Bills
                .Include(b => b.BillDetails)
                .Include(b => b.Consumer)
                .Include(b => b.Tariff)
                .FirstOrDefaultAsync(b => b.BillId == id);
        }

        public async Task<Bill> CreateAsync(Bill bill)
        {
            _context.Bills.Add(bill);
            await _context.SaveChangesAsync();
            return bill;
        }

        public async Task UpdateAsync(Bill bill)
        {
            _context.Bills.Update(bill);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var bill = await _context.Bills.FindAsync(id);
            if (bill != null)
            {
                _context.Bills.Remove(bill);
                await _context.SaveChangesAsync();
            }
        }
    }
}
