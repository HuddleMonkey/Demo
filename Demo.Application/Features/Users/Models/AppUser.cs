using Demo.Application.Features.Content.Library.Models;
using Demo.Application.Features.Teams.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Application.Features.Users.Models;

/// <summary>
/// Application User
/// </summary>
public class AppUser : IdentityUser
{
    /// <summary>
    /// First Name
    /// </summary>
    public string FirstName { get; set; } = "";

    /// <summary>
    /// Last Name
    /// </summary>
    public string LastName { get; set; } = "";

    /// <summary>
    /// Gets the full name of the user
    /// </summary>
    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>
    /// Refresh Token
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Date/time the refresh token expires
    /// </summary>
    public DateTime? RefreshTokenExpiryTime { get; set; }

    /// <summary>
    /// Date/time of the last user activity of when the user logged in or refreshed their token
    /// </summary>
    public DateTime? LastActivityTime { get; set; }

    /// <summary>
    /// Teams the user is a member of
    /// </summary>
    public List<Team> Teams { get; set; } = [];

    /// <summary>
    /// Associated organization user metadata
    /// </summary>
    [NotMapped]
    public OrganizationUser? OrganizationUser { get; set; } = null;

    /// <summary>
    /// Associated provider user metadata
    /// </summary>
    [NotMapped]
    public ProviderUser? ProviderUser { get; set; } = null;
}
