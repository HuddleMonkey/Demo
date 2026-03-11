using Demo.Application.Features.Users.Models;

namespace Demo.Application.Features.Events.Models;

/// <summary>
/// Event Schedule
/// </summary>
public class Schedule : BaseEntity
{
    /// <summary>
    /// Id of the associated series part
    /// </summary>
    public long SeriesPartId { get; set; }

    /// <summary>
    /// Id of the associated position
    /// </summary>
    public long? PositionId { get; set; }

    /// <summary>
    /// Optional name of the associated location
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// Optional time the position starts
    /// </summary>
    public TimeOnly? StartTime { get; set; }

    /// <summary>
    /// Optional time the position ends
    /// </summary>
    public TimeOnly? EndTime { get; set; }

    /// <summary>
    /// Optional category to group the position under
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Name of the position
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Optional description of the position
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Id of the scheduled user
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Scheduled User
    /// </summary>
    public virtual AppUser? User { get; set; }

    /// <summary>
    /// Status of the schedule request
    /// </summary>
    public ScheduleStatus Status { get; set; }

    /// <summary>
    /// Date/Time the user was invited, accepted, or declined
    /// </summary>
    public DateTime? StatusDate { get; set; }

    /// <summary>
    /// Optional reason the user declined the schedule request
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Optional other availability to serve if the user declined the schedule request
    /// </summary>
    public string? OtherAvailability { get; set; }
}
