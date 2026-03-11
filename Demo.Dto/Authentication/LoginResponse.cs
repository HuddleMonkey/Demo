namespace Demo.Dto.Authentication;

/// <summary>
/// Login response
/// </summary>
public class LoginResponse
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
