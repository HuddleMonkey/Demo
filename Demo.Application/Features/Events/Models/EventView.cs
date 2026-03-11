using Demo.Application.Features.Teams.Models;
using Demo.Application.Features.Users.Models;

namespace Demo.Application.Features.Events.Models;

/// <summary>
/// Event View
/// </summary>
public class EventView
{
    /// <summary>
    /// Event
    /// </summary>
    public Event Event { get; set; } = new();

    /// <summary>
    /// Users that can be granted permission to the event
    /// </summary>
    public List<AppUser> AvailableUsersForPermissions { get; set; } = [];

    /// <summary>
    /// Teams that can be assigned to a position
    /// </summary>
    public List<Team> Teams { get; set; } = [];

    /// <summary>
    /// Locations to create associated positions
    /// </summary>
    public List<Location> Locations { get; set; } = [];
}
