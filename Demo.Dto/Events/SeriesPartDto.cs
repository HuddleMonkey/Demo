using Demo.Shared.Constants;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Dto.Events;

/// <summary>
/// Series Part
/// </summary>
public class SeriesPartDto : BaseEntityDto
{
    /// <summary>
    /// Id of the associated series
    /// </summary>
    public long SeriesId { get; set; }

    /// <summary>
    /// Date of the series part
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Availability
    /// </summary>
    [NotMapped]
    public Availability Availability { get; set; }

    /// <summary>
    /// Optional ame of the series part
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Optional description of the series part
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Schedules associated with the series part
    /// </summary>
    public List<ScheduleDto> Schedules { get; set; } = [];
}
