using AMI_Project.Models;

namespace AMI_Project.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken token, CancellationToken ct);
        Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);
    }
}
