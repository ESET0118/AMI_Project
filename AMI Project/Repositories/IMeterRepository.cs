using AMI_Project.Data.Models;
using System.Threading;

namespace AMI_Project.Repositories.Interfaces
{
    public interface IMeterRepository
    {
        Task AddAsync(Meter meter, CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);
        Task<Meter?> GetByIdAsync(string meterSerialNo, CancellationToken ct);
        Task<IEnumerable<Meter>> GetAllAsync(CancellationToken ct);
    }
}
