using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Application.Features.Events.Models;

/// <summary>
/// Event
/// </summary>
public class Event : BaseAuditEntity
{
    /// <summary>
    /// The ID of the organization the event belongs to
    /// </summary>
    public long OrganizationId { get; set; }

    /// <summary>
    /// Name of the event
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Description of the event
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Optional URL of the logo for the event
    /// </summary>
    public string? LogoUrl { get; set; }

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

    /// <summary>
    /// Interval, in days, to send schedule reminders to those who have not yet responded. For example, if set to 7, another invitation 
    /// request for the schedule will be sent if there has been no response and it has been at least a week since being invited. This 
    /// will repeat until there is a response.
    /// </summary>
    public int RemindNotRespondedInterval { get; set; }

    /// <summary>
    /// Threshold of days until the event to start sending daily schedule reminders to those who have not yet responded to a schedule 
    /// request. For example, if set to 3, another invitation request for the schedule will be sent each day leading up to the event 
    /// until there is a response.
    /// </summary>
    public int RemindNotRespondedEventThreshold { get; set; }

    /// <summary>
    /// List of one or more days of when to send a reminder to those that have accepted a schedule for an event. List is 
    /// separated with a |, such as 3|5|7. Add 0 to send a reminder on the day of the event. For example, if set to 3 and 
    /// 5, a reminder will be sent 5 days prior to the event and 3 days prior to the event.
    /// </summary>
    public string RemindUpcomingSchedule { get; set; } = "";

    /// <summary>
    /// List of one or more days of when to send a reminder to those that have accepted a schedule for an event. Add 0 to 
    /// send a reminder on the day of the event. For example, if set to 3 and 5, a reminder will be sent 5 days prior to 
    /// the event and 3 days prior to the event.
    /// </summary>
    public List<int> RemindUpcomingScheduleDays => !string.IsNullOrWhiteSpace(RemindUpcomingSchedule) ? [.. RemindUpcomingSchedule.Split('|').Select(int.Parse).Order()] : [];

    /// <summary>
    /// Any positions that must filled for the event
    /// </summary>
    public virtual List<Position> Positions { get; set; } = [];

    /// <summary>
    /// Any schedule templates associated with the event
    /// </summary>
    public virtual List<ScheduleTemplate> ScheduleTemplates { get; set; } = [];

    /// <summary>
    /// Any series created for the event
    /// </summary>
    public virtual List<Series> Series { get; set; } = [];

    /// <summary>
    /// Permission level the user has to the event
    /// </summary>
    [NotMapped]
    public EventPermissionLevel PermissionLevel { get; set; }
}
