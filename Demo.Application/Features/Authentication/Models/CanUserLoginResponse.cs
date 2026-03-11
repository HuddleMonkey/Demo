using Demo.Application.Features.Content.Library.Models;
using Demo.Application.Features.Organizations.Models;

namespace Demo.Application.Features.Authentication.Models;

/// <summary>
/// Response to whether a user can login
/// </summary>
public class CanUserLoginResponse
{
    /// <summary>
    /// Valid organizations the user is associated with
    /// </summary>
    public List<Organization> Organizations { get; set; } = [];

    /// <summary>
    /// Valid providers the user is associated with
    /// </summary>
    public List<ProviderUser> Providers { get; set; } = [];
}
