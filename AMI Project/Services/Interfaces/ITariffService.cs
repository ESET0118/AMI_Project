using AMI_Project.Models;

namespace AMI_Project.Services.Interfaces
{
    public interface ITariffService
    {
        Task<IEnumerable<Tariff>> GetAllAsync(CancellationToken ct);
        Task<Tariff?> GetByIdAsync(int id, CancellationToken ct);
        Task<Tariff> CreateAsync(Tariff tariff, CancellationToken ct);
        Task<Tariff?> UpdateAsync(int id, Tariff tariff, CancellationToken ct);
        Task DeleteAsync(int id, CancellationToken ct);
    }
}
