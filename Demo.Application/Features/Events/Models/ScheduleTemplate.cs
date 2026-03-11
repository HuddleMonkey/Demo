using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Application.Features.Events.Models;

/// <summary>
/// Template that will be applied to a schedule
/// </summary>
public class ScheduleTemplate : BaseAuditEntity
{
    /// <summary>
    /// Id of the associated event
    /// </summary>
    public long EventId { get; set; }

    /// <summary>
    /// Name of the template
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Description of the template
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Template positions
    /// </summary>
    public virtual List<ScheduleTemplatePosition> TemplatePositions { get; set; } = [];

    /// <summary>
    /// Permission level the user has to the event
    /// </summary>
    [NotMapped]
    public EventPermissionLevel PermissionLevel { get; set; }

    /// <summary>
    /// Flag if the current user can edit this template
    /// </summary>
    [NotMapped]
    public bool CurrentUserCanEdit { get; set; }
}
