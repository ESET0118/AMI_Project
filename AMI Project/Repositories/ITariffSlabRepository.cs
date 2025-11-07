using AMI_Project.Models;

namespace AMI_Project.Repositories.Interfaces
{
    public interface ITariffSlabRepository
    {
        Task<IEnumerable<TariffSlab>> GetAllAsync(CancellationToken ct);
        Task<TariffSlab?> GetByIdAsync(int id, CancellationToken ct);
        Task<TariffSlab> AddAsync(TariffSlab slab, CancellationToken ct);
        Task<TariffSlab?> UpdateAsync(TariffSlab slab, CancellationToken ct);
        Task DeleteAsync(int id, CancellationToken ct);
    }
}
