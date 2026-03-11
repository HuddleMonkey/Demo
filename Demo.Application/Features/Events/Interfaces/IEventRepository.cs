using Demo.Application.Features.Events.Models;

namespace Demo.Application.Features.Events.Interfaces;

public interface IEventRepository : IRepository<Event>
{
    /// <summary>
    /// Get the events with the given ids
    /// </summary>
    /// <param name="ids">Ids of the events to retrieve</param>
    /// <param name="include">Properties to include</param>
    /// <returns>List of events</returns>
    Task<List<Event>> GetEventsAsync(List<long> ids, IncludeEventProperties include = IncludeEventProperties.None);

    /// <summary>
    /// Gets events the user created
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="include">Flags for what data to include. Default is None.</param>
    /// <returns>List of events</returns>
    Task<List<Event>> GetEventsUserCreatedAsync(string userId, IncludeEventProperties include = IncludeEventProperties.None);

    /// <summary>
    /// Gets events the user created
    /// </summary>
    /// <param name="organizationId">Id of the organization</param>
    /// <param name="userId">User ID</param>
    /// <param name="include">Flags for what data to include. Default is None.</param>
    /// <returns>List of events</returns>
    Task<List<Event>> GetEventsUserCreatedAsync(long organizationId, string userId, IncludeEventProperties include = IncludeEventProperties.None);

    /// <summary>
    /// Gets an event with the given id
    /// </summary>
    /// <param name="id">Id of the event</param>
    /// <param name="include">Flags for what data to include. Default is None.</param>
    /// <returns>Event or NULL if not found</returns>
    Task<Event?> GetEventAsync(long id, IncludeEventProperties include = IncludeEventProperties.None);
}
