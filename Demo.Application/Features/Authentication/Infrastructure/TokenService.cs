using Demo.Application.Domain.Settings;
using Demo.Application.Features.Authentication.Interfaces;
using Demo.Application.Features.Organizations.Models;
using Demo.Application.Features.Users.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace Demo.Application.Features.Authentication.Infrastructure;

public class TokenService(IOptions<DemoSettings> settings) : ITokenService
{
    private readonly DemoSettings _settings = settings.Value;

    /// <summary>
    /// Generates a token for the user
    /// </summary>
    /// <param name="user">User</param>
    /// <param name="claims">User claims</param>
    /// <param name="organizations">Organizations the user is associated with</param>
    /// <param name="isAdmin">Whether or not the user is a system admin</param>
    /// <returns>Token</returns>
    public string GenerateToken(AppUser user, IList<Claim> claims, List<Organization> organizations, bool isAdmin)
    {
        ClaimsIdentity claimsIdentity = GetUserClaims(user, claims, organizations, isAdmin);
        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = claimsIdentity,
            Issuer = _settings.Token.Issuer,
            Audience = _settings.Token.Audience,
            Expires = DateTime.UtcNow.AddMinutes(_settings.Token.TokenExpiryTimeInMinutes),
            NotBefore = DateTime.UtcNow,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Token.Key)), SecurityAlgorithms.HmacSha256)
        };

        string token = new JsonWebTokenHandler().CreateToken(tokenDescriptor);

        return token;
    }

    /// <summary>
    /// Generates a random refresh token
    /// </summary>
    /// <returns>Refresh token</returns>
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber);
    }

    /// <summary>
    /// Generates the time the refresh token expires
    /// </summary>
    /// <returns>DateTime the token expires</returns>
    public DateTime GetRefreshTokenExpiryTime() => DateTime.UtcNow.AddMinutes(_settings.Token.RefreshTokenExpiryTimeInMinutes);

    /// <summary>
    /// Gets the user id from the token
    /// </summary>
    /// <param name="token">Token</param>
    /// <returns>User Id</returns>
    public async Task<string> GetUserIdFromTokenAsync(string token)
    {
        TokenValidationParameters tokenValidationParameters = new()
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Token.Key)),
            ValidateLifetime = false,
            ValidIssuer = _settings.Token.Issuer,
            ValidAudience = _settings.Token.Audience
        };

        JsonWebTokenHandler tokenHandler = new();
        TokenValidationResult tokenValidationResult = await tokenHandler.ValidateTokenAsync(token, tokenValidationParameters);
        Claim? claim = tokenValidationResult.ClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        string userId = claim?.Value ?? "";

        return userId;
    }

    /// <summary>
    /// Gets the user's claims.
    /// </summary>
    /// <param name="user">User</param>
    /// <param name="claims">User claims</param>
    /// <param name="organizations">Organizations the user is associated with</param>
    /// <param name="isAdmin">Whether or not the user is an admin</param>
    /// <returns>ClaimsIdentity</returns>
    private ClaimsIdentity GetUserClaims(AppUser user, IList<Claim> claims, List<Organization> organizations, bool isAdmin)
    {
        ClaimsIdentity claimsIdentity = new(claims);
        claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
        claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, user.Email!));
        claimsIdentity.AddClaim(new Claim(ClaimTypes.Surname, user.LastName));
        claimsIdentity.AddClaim(new Claim(ClaimTypes.GivenName, user.FirstName));

        foreach (var organization in organizations.Where(o => o.Status != OrganizationStatus.Deleted))
        {
            claimsIdentity.AddClaim(new Claim(DemoClaimTypes.Organization, $"{organization.Id}-{organization.Name}"));
        }

        if (isAdmin)
        {
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, DemoRoles.Admin));
        }

        return claimsIdentity;
    }
}
