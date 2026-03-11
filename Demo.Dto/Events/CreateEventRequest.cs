using Demo.Shared.Constants;
using System.ComponentModel.DataAnnotations;

namespace Demo.Dto.Events;

/// <summary>
/// Request to create a new event
/// </summary>
public class CreateEventRequest
{
    /// <summary>
    /// Id of the organization to associate the event with
    /// </summary>
    public long OrganizationId { get; set; }

    /// <summary>
    /// Name of the event
    /// </summary>
    [Required(ErrorMessage = "Please provide a name for the event")]
    [StringLength(300, ErrorMessage = "{0} must be less than {1} characters long")]
    public string Name { get; set; } = "";

    /// <summary>
    /// Description of the event
    /// </summary>
    [StringLength(5000, ErrorMessage = "{0} must be less than {1} characters long")]
    public string? Description { get; set; } = "";

    /// <summary>
    /// If contains a value, the image to set as the logo.
    /// </summary>
    public string? ImageBase64 { get; set; }

    /// <summary>
    /// Whether or not the event is recurring and if so how often
    /// </summary>
    public Recurring Recurring { get; set; }

    /// <summary>
    /// If recurring is Custom, how often the event recurs
    /// </summary>
    public int? CustomFrequency { get; set; } = 1;

    /// <summary>
    /// If recurring is Custom, the time unit indicating when the event recurs
    /// </summary>
    public RecurringTimeUnit? CustomTimeUnit { get; set; } = RecurringTimeUnit.Days;

    /// <summary>
    /// If recurring is Custom and CustomTimeUnit is weekly, the day of the week the event recurs
    /// </summary>
    public DayOfWeek? CustomDayOfWeek { get; set; } = DayOfWeek.Sunday;

    /// <summary>
    /// Interval, in days, to send schedule reminders to those who have not yet responded.
    /// </summary>
    [Range(1, 100)]
    public int RemindNotRespondedInterval { get; set; } = 7;

    /// <summary>
    /// Threshold of days until the event to start sending daily schedule reminders to those who have not yet responded to a schedule request.
    /// </summary>
    [Range(1, 100)]
    public int RemindNotRespondedEventThreshold { get; set; } = 3;

    /// <summary>
    /// List of one or more days of when to send a reminder to those that have accepted a schedule for an event. Add 0 to send a reminder 
    /// on the day of the event.
    /// </summary>
    public List<int> RemindUpcomingScheduleDays { get; set; } = [3];

    /// <summary>
    /// User ids to give permissions to
    /// </summary>
    public List<string> PermissionUserIds { get; set; } = [];

    /// <summary>
    /// Optional positions to associate with the event
    /// </summary>
    public List<PositionDto> Positions { get; set; } = [];
}
