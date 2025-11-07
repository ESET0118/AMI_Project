using AMI_Project.Data;
using AMI_Project.DTOs.Auth;
using AMI_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AMI_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AMIDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AMIDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // -------------------------------
        // 🟢 Register (Sign Up)
        // -------------------------------
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto, CancellationToken ct)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email, ct))
                return BadRequest(new { message = "Email already registered" });

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == dto.Role, ct);
            if (role == null)
                return BadRequest(new { message = $"Invalid role: {dto.Role}" });

            var user = new User
            {
                Email = dto.Email,
                PasswordHash = dto.Password, 
                DisplayName = dto.DisplayName,
                Phone = dto.Phone,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };

            user.Roles.Add(role);

            _context.Users.Add(user);
            await _context.SaveChangesAsync(ct);

            return Ok(new { message = "User registered successfully" });
        }

        // -------------------------------
        // 🔵 Login (Sign In)
        // -------------------------------
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken ct)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Email == dto.Email, ct);

            if (user == null || user.PasswordHash != dto.Password)
                return Unauthorized(new { message = "Invalid email or password" });

            var token = GenerateJwtToken(user);

            return Ok(new
            {
                message = "Login successful",
                token,
                user = new
                {
                    user.UserId,
                    user.Email,
                    user.DisplayName,
                    Roles = user.Roles.Select(r => r.Name).ToList()
                }
            });
        }

        // -------------------------------
        // 🟣 Helper: Generate JWT Token
        // -------------------------------
        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            // Add role claims
            claims.AddRange(user.Roles.Select(r => new Claim(ClaimTypes.Role, r.Name)));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3), // token valid for 3 hours
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
