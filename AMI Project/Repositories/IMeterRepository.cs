using AMI_Project.Models;
using AMI_Project.Helpers;

namespace AMI_Project.Repositories
{
    public interface IMeterRepository
    {
        Task<PagedResult<Meter>> GetMetersAsync(
            string? serialNo,
            string? status,
            long? consumerId,
            DateTime? fromInstall,
            DateTime? toInstall,
            int page,
            int pageSize,
            CancellationToken ct);

        Task<Meter?> GetBySerialAsync(string serialNo, CancellationToken ct);
        Task AddAsync(Meter meter, CancellationToken ct);
        Task UpdateAsync(Meter meter, CancellationToken ct);
        Task DeleteAsync(string serialNo, CancellationToken ct);

        Task AddOrUpdateBulkAsync(IEnumerable<Meter> meters, CancellationToken ct);
        Task<bool> ConsumerExistsAsync(long consumerId, CancellationToken ct);
    }
}
