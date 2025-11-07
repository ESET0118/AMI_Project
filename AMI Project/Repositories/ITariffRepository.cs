using AMI_Project.Models;

namespace AMI_Project.Repositories.Interfaces
{
    public interface ITariffRepository
    {
        Task<IEnumerable<Tariff>> GetAllAsync(CancellationToken ct);
        Task<Tariff?> GetByIdAsync(int id, CancellationToken ct);
        Task<Tariff> AddAsync(Tariff tariff, CancellationToken ct);
        Task<Tariff?> UpdateAsync(Tariff tariff, CancellationToken ct);
        Task DeleteAsync(int id, CancellationToken ct);
    }
}
