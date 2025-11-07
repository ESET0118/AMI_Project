using AMI_Project.DTOs.Auth;
using AMI_Project.DTOs.Users;

namespace AMI_Project.Services.Interfaces
{
    public interface IAuthService
    {
        Task<UserAuthResponseDto> RegisterAsync(RegisterDto dto, CancellationToken ct);
        Task<UserAuthResponseDto> LoginAsync(LoginDto dto, CancellationToken ct);
        Task<UserAuthResponseDto> RefreshTokenAsync(RefreshTokenDto dto, CancellationToken ct);
    }
}
