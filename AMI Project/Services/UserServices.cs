using AMI_Project.Data;
using AMI_Project.Models;
using AMI_Project.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AMI_Project.Services
{
    public class UserServices : IUserServices
    {
        private readonly AMIDbContext _context;
        public UserServices(AMIDbContext context) => _context = context;

        public async Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken ct)
        {
            return await _context.Users.Include(u => u.Roles).ToListAsync(ct);
        }

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

        public void Delete(User user)
        {
            _context.Users.Remove(user);
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
        }
    }
}
