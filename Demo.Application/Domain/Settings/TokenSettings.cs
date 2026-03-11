namespace Demo.Application.Domain.Settings;

/// <summary>
/// Token settings
/// </summary>
public class TokenSettings
{
    /// <summary>
    /// Issuer of the token
    /// </summary>
    public string Issuer { get; set; } = "";

    /// <summary>
    /// Audience of the token
    /// </summary>
    public string Audience { get; set; } = "";

    /// <summary>
    /// Key used to sign the token
    /// </summary>
    public string Key { get; set; } = "";

    /// <summary>
    /// Number of minutes before a token expires
    /// </summary>
    public double TokenExpiryTimeInMinutes { get; set; }

    /// <summary>
    /// Number of minutes before a refresh token expires
    /// </summary>
    public double RefreshTokenExpiryTimeInMinutes { get; set; }
}
