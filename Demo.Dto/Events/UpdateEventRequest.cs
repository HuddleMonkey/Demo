using Demo.Shared.Constants;
using System.ComponentModel.DataAnnotations;

namespace Demo.Dto.Events;

/// <summary>
/// Request to update an event
/// </summary>
public class UpdateEventRequest
{
    /// <summary>
    /// Id of the event to update
    /// </summary>
    public long EventId { get; set; }

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
    /// Whether or not the event is recurring and if so how often
    /// </summary>
    public Recurring Recurring { get; set; }

    /// <summary>
    /// If recurring is Custom, how often the event recurs
    /// </summary>
    public int? CustomFrequency { get; set; }

    /// <summary>
    /// If recurring is Custom, the time unit indicating when the event recurs
    /// </summary>
    public RecurringTimeUnit? CustomTimeUnit { get; set; }

    /// <summary>
    /// If recurring is Custom and CustomTimeUnit is weekly, the day of the week the event recurs
    /// </summary>
    public DayOfWeek? CustomDayOfWeek { get; set; }
}
