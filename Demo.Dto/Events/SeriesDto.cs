using Demo.Shared.Constants;

namespace Demo.Dto.Events;

/// <summary>
/// Event Series
/// </summary>
public class SeriesDto : BaseEntityDto
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
    /// Parts associated with the series
    /// </summary>
    public List<SeriesPartDto> Parts { get; set; } = [];
}
