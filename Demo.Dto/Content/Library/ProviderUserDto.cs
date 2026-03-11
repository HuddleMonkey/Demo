using Demo.Shared.Constants;

namespace Demo.Dto.Content.Library;

/// <summary>
/// Metadata of a user associated with a provider
/// </summary>
public class ProviderUserDto
{
    /// <summary>
    /// Status of the user's account
    /// </summary>
    public UserStatus Status { get; set; }

    /// <summary>
    /// Date/Time the user was created
    /// </summary>
    public DateTime? DateCreated { get; set; }
}
