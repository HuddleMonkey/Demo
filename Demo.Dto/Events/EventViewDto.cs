using Demo.Dto.Teams;
using Demo.Dto.Users;

namespace Demo.Dto.Events;

/// <summary>
/// Event View
/// </summary>
public class EventViewDto
{
    /// <summary>
    /// Event
    /// </summary>
    public EventDto Event { get; set; } = new();

    /// <summary>
    /// Users that can be granted permission to the event
    /// </summary>
    public List<AppUserDto> AvailableUsersForPermissions { get; set; } = [];

    /// <summary>
    /// Teams that can be assigned to a position
    /// </summary>
    public List<TeamDto> Teams { get; set; } = [];

    /// <summary>
    /// Locations to create associated positions
    /// </summary>
    public List<LocationDto> Locations { get; set; } = [];
}
