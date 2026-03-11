using Demo.Dto.Teams;
using Demo.Dto.Users;

namespace Demo.Dto.Events;

/// <summary>
/// Events View
/// </summary>
public class EventsViewDto
{
    /// <summary>
    /// Organization Id
    /// </summary>
    public long OrganizationId { get; set; }

    /// <summary>
    /// Events
    /// </summary>
    public List<EventDto> Events { get; set; } = [];

    /// <summary>
    /// Users that can be granted permission to the project
    /// </summary>
    public List<AppUserDto> AvailableUsersForPermissions { get; set; } = [];

    /// <summary>
    /// Existing locations used for adding positions to a new event
    /// </summary>
    public List<LocationDto> Locations { get; set; } = [];

    /// <summary>
    /// Teams that can be assigned to a position
    /// </summary>
    public List<TeamDto> Teams { get; set; } = [];
}
