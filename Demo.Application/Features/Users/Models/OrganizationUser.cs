namespace Demo.Application.Features.Users.Models;

/// <summary>
/// Metadata of a user associated with an organization
/// </summary>
public class OrganizationUser : BaseEntity
{
    /// <summary>
    /// The ID of the organization the user belongs to
    /// </summary>
    public long OrganizationId { get; set; }

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
    /// If set, the Date/Time to auto delete the user
    /// </summary>
    public DateTime? AutoDeleteOn { get; set; }

    /// <summary>
    /// Date/Time the user was deleted
    /// </summary>
    public DateTime? DateDeleted { get; set; }

    /// <summary>
    /// Date/time of the last user activity of when the user logged in or refreshed their token
    /// </summary>
    public DateTime? LastActivityTime { get; set; }
}
