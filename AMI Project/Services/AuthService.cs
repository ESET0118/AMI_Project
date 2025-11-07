using AMI_Project.Services.Interfaces;
using AMI_Project.DTOs.Auth;
using AMI_Project.DTOs.Users;
using AMI_Project.Data;
using AMI_Project.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AMI_Project.Services
{
    public class AuthService : IAuthService
    {
        private readonly AMIDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(AMIDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // ✅ REGISTER
        public async Task<UserAuthResponseDto> RegisterAsync(RegisterDto dto, CancellationToken ct)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email, ct))
                throw new Exception("Email already registered.");

            var user = new User
            {
                Email = dto.Email,
                PasswordHash = dto.Password, // plain password for simplicity
                DisplayName = dto.DisplayName,
                Phone = dto.Phone,
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = true
            };

            // ensure role exists
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == dto.Role, ct);
            if (role == null)
            {
                role = new Role { Name = dto.Role };
                _context.Roles.Add(role);
            }

            user.Roles.Add(role);
            _context.Users.Add(user);
            await _context.SaveChangesAsync(ct);

            // create access token immediately after register
            var tokens = GenerateJwtTokens(user);

            return new UserAuthResponseDto
            {
                UserId = user.UserId,
                Email = user.Email,
                DisplayName = user.DisplayName,
                Role = dto.Role,
                AccessToken = tokens.accessToken,
                RefreshToken = tokens.refreshToken
            };
        }

        // ✅ LOGIN
        public async Task<UserAuthResponseDto> LoginAsync(LoginDto dto, CancellationToken ct)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Email == dto.Email && u.PasswordHash == dto.Password, ct);

            if (user == null)
                throw new Exception("Invalid credentials.");

            var tokens = GenerateJwtTokens(user);

            return new UserAuthResponseDto
            {
                UserId = user.UserId,
                Email = user.Email,
                DisplayName = user.DisplayName,
                Role = user.Roles.FirstOrDefault()?.Name,
                AccessToken = tokens.accessToken,
                RefreshToken = tokens.refreshToken
            };
        }

        // 🚫 Not implemented (for now)
        public Task<UserAuthResponseDto> RefreshTokenAsync(RefreshTokenDto dto, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        // 🔐 PRIVATE: JWT Generator
        private (string accessToken, string refreshToken) GenerateJwtTokens(User user)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            foreach (var role in user.Roles)
                claims.Add(new Claim(ClaimTypes.Role, role.Name));

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiryMinutes"])),
                signingCredentials: creds
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            var refreshToken = Guid.NewGuid().ToString(); // 🔄 simple GUID refresh token placeholder

            return (accessToken, refreshToken);
        }
    }
}
