using AMI_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace AMI_Project.Services.Interfaces
{
    public interface IUserServices
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken ct);
        Task<User?> GetByIdAsync(long id, CancellationToken ct);
        Task AddAsync(User user, CancellationToken ct);
        Task<bool> EmailExistsAsync(string email, CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);
        Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken ct);


    }
}
