using AMI_Project.Data;
using AMI_Project.Models;
using AMI_Project.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AMI_Project.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AMIDbContext _context;

        public RefreshTokenRepository(AMIDbContext context)
        {
            _context = context;
        }

        // ✅ Add a new refresh token
        public async Task AddAsync(RefreshToken token, CancellationToken ct)
        {
            await _context.RefreshTokens.AddAsync(token, ct);
        }

        // ✅ Find a refresh token by token string (includes User for Auth)
        public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct)
        {
            return await _context.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == token, ct);
        }

        // ✅ Save all pending changes
        public async Task SaveChangesAsync(CancellationToken ct)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}
