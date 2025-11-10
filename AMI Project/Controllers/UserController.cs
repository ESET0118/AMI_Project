using AMI_Project.Models;
using AMI_Project.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AMI_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Protect all endpoints with JWT
    public class UsersController : ControllerBase
    {
        private readonly IUserServices _userService;

        public UsersController(IUserServices userService)
        {
            _userService = userService;
        }

        // GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetUsers(CancellationToken ct)
        {
            var users = await _userService.GetAllUsersAsync(ct); // We'll add this method
            var result = users.Select(u => new
            {
                u.UserId,
                u.Email,
                u.DisplayName,
                Role = u.Roles.Select(r => r.Name).FirstOrDefault() ?? "N/A",
                u.CreatedAt
            });

            return Ok(result);
        }

        // GET: api/users/{id}
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetUser(long id, CancellationToken ct)
        {
            var user = await _userService.GetByIdAsync(id, ct);
            if (user == null) return NotFound();

            return Ok(new
            {
                user.UserId,
                user.Email,
                user.DisplayName,
                Role = user.Roles.Select(r => r.Name).FirstOrDefault() ?? "N/A",
                user.CreatedAt
            });
        }

        // POST: api/users
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User user, CancellationToken ct)
        {
            if (await _userService.EmailExistsAsync(user.Email, ct))
                return BadRequest(new { Message = "Email already exists." });

            await _userService.AddAsync(user, ct);
            await _userService.SaveChangesAsync(ct);

            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
        }
    }
}
