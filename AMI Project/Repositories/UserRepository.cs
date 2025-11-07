using AMI_Project.Data;
using AMI_Project.Models;
using AMI_Project.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AMI_Project.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AMIDbContext _context;
        public UserRepository(AMIDbContext context) => _context = context;

        public async Task<User?> GetByEmailAsync(string email, CancellationToken ct)
            => await _context.Users.Include(u => u.Roles)
                                   .FirstOrDefaultAsync(u => u.Email == email, ct);

        public async Task<User?> GetByIdAsync(long id, CancellationToken ct)
            => await _context.Users.Include(u => u.Roles)
                                   .FirstOrDefaultAsync(u => u.UserId == id, ct);

        public async Task AddAsync(User user, CancellationToken ct)
            => await _context.Users.AddAsync(user, ct);

        public async Task<bool> EmailExistsAsync(string email, CancellationToken ct)
            => await _context.Users.AnyAsync(u => u.Email == email, ct);

        public async Task SaveChangesAsync(CancellationToken ct)
            => await _context.SaveChangesAsync(ct);
    }
}
