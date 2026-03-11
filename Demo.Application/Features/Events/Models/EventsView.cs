using Demo.Application.Features.Teams.Models;
using Demo.Application.Features.Users.Models;

namespace Demo.Application.Features.Events.Models;

/// <summary>
/// Events View
/// </summary>
public class EventsView
{
    /// <summary>
    /// Organization Id
    /// </summary>
    public long OrganizationId { get; set; }

    /// <summary>
    /// Events
    /// </summary>
    public List<Event> Events { get; set; } = [];

    /// <summary>
    /// Users that can be granted permission to the project
    /// </summary>
    public List<AppUser> AvailableUsersForPermissions { get; set; } = [];

    /// <summary>
    /// Existing locations used for adding positions to a new event
    /// </summary>
    public List<Location> Locations { get; set; } = [];

    /// <summary>
    /// Teams that can be assigned to a position
    /// </summary>
    public List<Team> Teams { get; set; } = [];
}
