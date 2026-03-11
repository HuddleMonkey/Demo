namespace Demo.Dto.Events;

/// <summary>
/// Event Position
/// </summary>
public class PositionDto : BaseEntityDto
{
    /// <summary>
    /// Optional id of the associated location for the position
    /// </summary>
    public long? LocationId { get; set; }

    /// <summary>
    /// Optional associated location for the position
    /// </summary>
    public LocationDto? Location { get; set; }

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
    /// Number of slots that need to be filled for the position
    /// </summary>
    public int NumberRequired { get; set; }
}
