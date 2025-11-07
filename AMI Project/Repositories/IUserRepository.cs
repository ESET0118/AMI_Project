using AMI_Project.Models;

namespace AMI_Project.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken ct);
        Task<User?> GetByIdAsync(long id, CancellationToken ct);
        Task AddAsync(User user, CancellationToken ct);
        Task<bool> EmailExistsAsync(string email, CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);
    }
}
