namespace Demo.Application.Features.Content.Library.Models;

/// <summary>
/// Metadata of a user associated with a provider
/// </summary>
public class ProviderUser : BaseEntity
{
    /// <summary>
    /// The ID of the provider the user belongs to
    /// </summary>
    public long ProviderId { get; set; }

    /// <summary>
    /// The ID of the user
    /// </summary>
    public string UserId { get; set; } = "";

    /// <summary>
    /// Status of the user's account
    /// </summary>
    public UserStatus Status { get; set; }

    /// <summary>
    /// Date/Time the user was created
    /// </summary>
    public DateTime? DateCreated { get; set; }

    /// <summary>
    /// Date/Time the user was deleted
    /// </summary>
    public DateTime? DateDeleted { get; set; }
}
