using Demo.Application.Features.Organizations.Models;
using Demo.Application.Features.Users.Models;

namespace Demo.Application.Features.Authentication.Interfaces;

public interface ITokenService
{
    /// <summary>
    /// Generates a token for the user
    /// </summary>
    /// <param name="user">User</param>
    /// <param name="claims">User claims</param>
    /// <param name="organizations">Organizations the user is associated with</param>
    /// <param name="isAdmin">Whether or not the user is a system admin</param>
    /// <returns>Token</returns>
    string GenerateToken(AppUser user, IList<Claim> claims, List<Organization> organizations, bool isAdmin);

    /// <summary>
    /// Generates a random refresh token
    /// </summary>
    /// <returns>Refresh token</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Generates the time the refresh token expires
    /// </summary>
    /// <returns>DateTime the token expires</returns>
    DateTime GetRefreshTokenExpiryTime();

    /// <summary>
    /// Gets the user id from the token
    /// </summary>
    /// <param name="token">Token</param>
    /// <returns>User Id</returns>
    Task<string> GetUserIdFromTokenAsync(string token);
}
