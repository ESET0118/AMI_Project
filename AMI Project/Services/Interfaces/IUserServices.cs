using AMI_Project.Models;

namespace AMI_Project.Services.Interfaces
{
    public interface IUserServices
    {
        Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken ct);
        Task<User?> GetByIdAsync(long id, CancellationToken ct);
        Task<User?> GetByEmailAsync(string email, CancellationToken ct);
        Task AddAsync(User user, CancellationToken ct);
        Task<bool> EmailExistsAsync(string email, CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);
        void Delete(User user);
        void Update(User user);
    }
}
