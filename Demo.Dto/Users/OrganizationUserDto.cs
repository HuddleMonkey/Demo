using Demo.Shared.Constants;

namespace Demo.Dto.Users;

/// <summary>
/// Metadata of a user associated with an organization
/// </summary>
public class OrganizationUserDto
{
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
