using Demo.Application.Features.Users.Models;

namespace Demo.Application.Features.Events.Models;

/// <summary>
/// User and position that will be set when the template is applied
/// </summary>
public class ScheduleTemplatePosition : BaseEntity
{
    /// <summary>
    /// Id of the associated schedule template
    /// </summary>
    public long ScheduleTemplateId { get; set; }

    /// <summary>
    /// Id of the associated postion
    /// </summary>
    public long PositionId { get; set; }

    /// <summary>
    /// Id of the user that will be scheduled for this position
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// User that will be scheduled for this position
    /// </summary>
    public virtual AppUser? User { get; set; }
}
