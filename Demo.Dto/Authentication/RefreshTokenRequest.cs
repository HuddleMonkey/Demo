namespace Demo.Dto.Authentication;

/// <summary>
/// Refresh token request
/// </summary>
public class RefreshTokenRequest
{
    /// <summary>
    /// Current token
    /// </summary>
    public string Token { get; set; } = "";

    /// <summary>
    /// Refresh token
    /// </summary>
    public string RefreshToken { get; set; } = "";
}
