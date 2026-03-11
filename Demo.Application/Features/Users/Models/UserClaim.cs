namespace Demo.Application.Features.Users.Models;

/// <summary>
/// Model for user claims.
/// </summary>
[Keyless]
public class UserClaim
{
    /// <summary>
    /// User ID
    /// </summary>
    public string UserId { get; set; } = "";

    /// <summary>
    /// Claim Type
    /// </summary>
    public string ClaimType { get; set; } = "";

    /// <summary>
    /// Claim Value
    /// </summary>
    public string ClaimValue { get; set; } = "";
}
