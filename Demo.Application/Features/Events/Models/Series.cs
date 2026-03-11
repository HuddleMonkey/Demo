using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Application.Features.Events.Models;

/// <summary>
/// Event Series
/// </summary>
public class Series : BaseEntity
{
    /// <summary>
    /// Id of the associated event
    /// </summary>
    public long EventId { get; set; }

    /// <summary>
    /// Date the series starts
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Date the series ends
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Availability
    /// </summary>
    [NotMapped]
    public Availability Availability { get; set; }

    /// <summary>
    /// Name of the series
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Optional description of the series
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Optional URL of the logo for the series
    /// </summary>
    public string? LogoUrl { get; set; }

    /// <summary>
    /// Optional id of the associated project
    /// </summary>
    public long? ProjectId { get; set; }

    /// <summary>
    /// Optional id of the associated conversation
    /// </summary>
    public long? ConversationId { get; set; }

    /// <summary>
    /// Parts associated with the series
    /// </summary>
    public virtual List<SeriesPart> Parts { get; set; } = [];
}
