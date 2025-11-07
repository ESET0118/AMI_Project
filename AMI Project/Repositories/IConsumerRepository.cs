using AMI_Project.Models;

namespace AMI_Project.Repositories.Interfaces
{
    public interface IConsumerRepository
    {
        Task<IEnumerable<Consumer>> GetAllAsync(CancellationToken ct);
        Task<Consumer?> GetByIdAsync(long id, CancellationToken ct);
        Task<Consumer> AddAsync(Consumer consumer, CancellationToken ct);
        Task<Consumer?> UpdateAsync(Consumer consumer, CancellationToken ct);
        Task DeleteAsync(long id, CancellationToken ct);
    }
}
