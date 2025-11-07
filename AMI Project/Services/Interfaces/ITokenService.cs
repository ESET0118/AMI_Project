using AMI_Project.Models;
using AMI_Project.DTOs.Auth;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMI_Project.Services
{
    public interface ITokenService
    {
        /// <summary>
        /// Generates a new JWT access token for a user with specific roles.
        /// </summary>
        string GenerateAccessToken(User user, List<string> roles);

        /// <summary>
        /// Creates and stores a new refresh token for the user.
        /// </summary>
        Task<RefreshToken> CreateRefreshTokenAsync(long userId, string ipAddress);

        /// <summary>
        /// Retrieves a valid (non-expired, non-revoked) refresh token from the database.
        /// </summary>
        Task<RefreshToken?> GetValidRefreshTokenAsync(string token);

        /// <summary>
        /// Revokes an existing refresh token.
        /// </summary>
        Task RevokeRefreshTokenAsync(RefreshToken refreshToken, string ipAddress, string? reason = null);
    }
}
