using AMI_Project.Data;
using AMI_Project.Models;
using AMI_Project.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AMI_Project.Repositories
{
    public class OrgUnitRepository : IOrgUnitRepository
    {
        private readonly AMIDbContext _context;

        public OrgUnitRepository(AMIDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrgUnit>> GetAllAsync(CancellationToken ct)
        {
            return await _context.OrgUnits
                .AsNoTracking()
                .Include(o => o.InverseParent)
                .ToListAsync(ct);
        }

        public async Task<OrgUnit?> GetByIdAsync(int id, CancellationToken ct)
        {
            return await _context.OrgUnits
                .Include(o => o.InverseParent)
                .FirstOrDefaultAsync(o => o.OrgUnitId == id, ct);
        }

        public async Task<OrgUnit> AddAsync(OrgUnit orgUnit, CancellationToken ct)
        {
            _context.OrgUnits.Add(orgUnit);
            await _context.SaveChangesAsync(ct);
            return orgUnit;
        }

        public async Task<OrgUnit?> UpdateAsync(OrgUnit orgUnit, CancellationToken ct)
        {
            var existing = await _context.OrgUnits.FindAsync(new object[] { orgUnit.OrgUnitId }, ct);
            if (existing == null) return null;

            _context.Entry(existing).CurrentValues.SetValues(orgUnit);
            await _context.SaveChangesAsync(ct);
            return existing;
        }

        public async Task DeleteAsync(int id, CancellationToken ct)
        {
            var orgUnit = await _context.OrgUnits.FindAsync(new object[] { id }, ct);
            if (orgUnit == null) return;

            _context.OrgUnits.Remove(orgUnit);
            await _context.SaveChangesAsync(ct);
        }
    }
}
