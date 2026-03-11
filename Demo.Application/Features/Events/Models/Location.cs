namespace Demo.Application.Features.Events.Models;

/// <summary>
/// Event Location
/// </summary>
public class Location : BaseEntity
{
    /// <summary>
    /// Id of the organization the location is associated with
    /// </summary>
    public long OrganizationId { get; set; }

    /// <summary>
    /// Name of the location
    /// </summary>
    public string Name { get; set; } = "";
}
