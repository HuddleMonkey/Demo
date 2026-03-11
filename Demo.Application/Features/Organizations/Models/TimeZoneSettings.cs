namespace Demo.Application.Features.Organizations.Models;

/// <summary>
/// Time zone settings for an organization
/// </summary>
public class TimeZoneSettings
{
    /// <summary>
    /// Todays date based on the organization's time zone
    /// </summary>
    public DateTime Today { get; set; }

    /// <summary>
    /// Organization's time zone id
    /// </summary>
    public string? TimeZone { get; set; }
}
