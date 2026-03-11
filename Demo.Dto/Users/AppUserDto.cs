using Demo.Dto.Content.Library;
using System.Text.Json.Serialization;

namespace Demo.Dto.Users;

/// <summary>
/// Application User
/// </summary>
public class AppUserDto
{
    /// <summary>
    /// User Id
    /// </summary>
    public string Id { get; set; } = "";

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
    [JsonIgnore]
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>
    /// Email
    /// </summary>
    public string Email { get; set; } = "";

    /// <summary>
    /// Date/Time the user was created
    /// </summary>
    public DateTime? DateCreated { get; set; }

    /// <summary>
    /// Associated organization user metadata
    /// </summary>
    public OrganizationUserDto? OrganizationUser { get; set; } = null;

    /// <summary>
    /// Associated provider user metadata
    /// </summary>
    public ProviderUserDto? ProviderUser { get; set; } = null;
}
