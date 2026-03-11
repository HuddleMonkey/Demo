using Demo.Application.Features.Events.Models;

namespace Demo.Application.Features.Events.Interfaces;

public interface IPositionRepository : IRepository<Position>
{
    /// <summary>
    /// Gets the positions for an event
    /// </summary>
    /// <param name="eventId">Id of the event</param>
    /// <param name="include">Flags for what data to include</param>
    /// <returns>List of Positions</returns>
    Task<List<Position>> GetPositionsAsync(long eventId, IncludePositionProperties include = IncludePositionProperties.None);
}
