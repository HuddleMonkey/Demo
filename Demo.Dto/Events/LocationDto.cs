namespace Demo.Dto.Events;

/// <summary>
/// Event Location
/// </summary>
public class LocationDto : BaseEntityDto
{
    /// <summary>
    /// Name of the location
    /// </summary>
    public string Name { get; set; } = "";
}