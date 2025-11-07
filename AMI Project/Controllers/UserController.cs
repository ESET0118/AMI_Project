using AMI_Project.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AMI_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // ✅ Protects endpoint with JWT
    public class UsersController : ControllerBase
    {
        private readonly AMIDbContext _context;

        public UsersController(AMIDbContext context)
        {
            _context = context;
        }

        // -------------------------------
        // GET: api/users
        // -------------------------------
        [HttpGet]
        public async Task<IActionResult> GetUsers(CancellationToken ct)
        {
            var users = await _context.Users
                .Include(u => u.Roles)
                .Select(u => new
                {
                    u.UserId,
                    u.Email,
                    u.DisplayName,
                    Role = u.Roles.Select(r => r.Name).FirstOrDefault() ?? "N/A",
                    u.CreatedAt
                })
                .ToListAsync(ct);

            return Ok(users);
        }
    }
}
