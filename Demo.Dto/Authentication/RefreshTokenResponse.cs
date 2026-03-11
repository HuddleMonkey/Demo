namespace Demo.Dto.Authentication;

/// <summary>
/// Refresh token response
/// </summary>
public class RefreshTokenResponse
{
    /// <summary>
    /// Token
    /// </summary>
    public string Token { get; set; } = "";

    /// <summary>
    /// Refresh token
    /// </summary>
    public string RefreshToken { get; set; } = "";
}
