using AMI_Project.Models;

namespace AMI_Project.Repositories.Interfaces
{
    public interface IOrgUnitRepository
    {
        Task<IEnumerable<OrgUnit>> GetAllAsync(CancellationToken ct);
        Task<OrgUnit?> GetByIdAsync(int id, CancellationToken ct);
        Task<OrgUnit> AddAsync(OrgUnit orgUnit, CancellationToken ct);
        Task<OrgUnit?> UpdateAsync(OrgUnit orgUnit, CancellationToken ct);
        Task DeleteAsync(int id, CancellationToken ct);
    }
}
