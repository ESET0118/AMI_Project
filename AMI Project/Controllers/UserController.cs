using AMI_Project.Data;
using AMI_Project.DTOs.Users;
using AMI_Project.Models;
using AMI_Project.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AMI_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserServices _userService;
        private readonly AMIDbContext _context;

        public UsersController(IUserServices userService, AMIDbContext context)
        {
            _userService = userService;
            _context = context;
        }

        // ✅ GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var users = await _userService.GetAllUsersAsync(ct);

            // Map to DTO to avoid circular reference issues
            var userDtos = users.Select(u => new
            {
                userId = u.UserId,
                email = u.Email,
                displayName = u.DisplayName,
                role = u.Roles.FirstOrDefault()?.Name ?? "User",
                createdAt = u.CreatedAt
            });

            return Ok(userDtos);
        }

        // ✅ GET: api/users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id, CancellationToken ct)
        {
            var user = await _userService.GetByIdAsync(id, ct);
            if (user == null)
                return NotFound(new { message = "User not found" });

            var dto = new
            {
                userId = user.UserId,
                email = user.Email,
                displayName = user.DisplayName,
                role = user.Roles.FirstOrDefault()?.Name ?? "User",
                createdAt = user.CreatedAt
            };

            return Ok(dto);
        }

        // ✅ PUT: api/users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(long id, [FromBody] UserUpdateDto dto, CancellationToken ct)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.UserId == id, ct);

            if (user == null)
                return NotFound(new { message = "User not found" });

            if (!string.IsNullOrWhiteSpace(dto.DisplayName))
                user.DisplayName = dto.DisplayName;

            if (!string.IsNullOrWhiteSpace(dto.Phone))
                user.Phone = dto.Phone;

            if (dto.EmailConfirmed.HasValue)
                user.EmailConfirmed = dto.EmailConfirmed.Value;

            if (!string.IsNullOrWhiteSpace(dto.Role))
            {
                user.Roles.Clear();
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == dto.Role, ct);
                if (role != null)
                    user.Roles.Add(role);
            }

            _userService.Update(user);
            await _userService.SaveChangesAsync(ct);

            return Ok(new { message = "User updated successfully." });
        }

        // ✅ DELETE: api/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id, CancellationToken ct)
        {
            var user = await _userService.GetByIdAsync(id, ct);
            if (user == null)
                return NotFound(new { message = "User not found" });

            _userService.Delete(user);
            await _userService.SaveChangesAsync(ct);

            return Ok(new { message = "User deleted successfully." });
        }
    }
}
