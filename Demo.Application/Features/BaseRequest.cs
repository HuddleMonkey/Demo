namespace Demo.Application.Features;

/// <summary>
/// Base request for MediatR
/// </summary>
public abstract class BaseRequest
{
    /// <summary>
    /// Current user making the request
    /// </summary>
    public ClaimsPrincipal? CurrentUser { get; set; }

    /// <summary>
    /// Current user id
    /// </summary>
    public string CurrentUserId => CurrentUser?.GetUserId() ?? string.Empty;

    /// <summary>
    /// Base URL
    /// </summary>
    public string BaseWebUrl { get; set; } = "";
}
