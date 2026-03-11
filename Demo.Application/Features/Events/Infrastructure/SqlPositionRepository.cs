using Demo.Application.Features.Events.Interfaces;
using Demo.Application.Features.Events.Models;

namespace Demo.Application.Features.Events.Infrastructure;

public class SqlPositionRepository(DemoDbContext context, ILogger<SqlPositionRepository> repoLogger) : Repository<Position>(context, repoLogger), IPositionRepository
{
    /// <summary>
    /// Gets the positions for an event
    /// </summary>
    /// <param name="eventId">Id of the event</param>
    /// <param name="include">Flags for what data to include</param>
    /// <returns>List of Positions</returns>
    public async Task<List<Position>> GetPositionsAsync(long eventId, IncludePositionProperties include = IncludePositionProperties.None)
    {
        logger.LogDebug($"Params: eventId={eventId}");

        IQueryable<Position> query = GetBaseQuery(include).Where(p => p.EventId == eventId);
        List<Position> positions = await query.AsSplitQuery().ToListAsync();

        positions = [.. positions
            .OrderBy(p => p.Location?.Name)
            .ThenBy(p => p.StartTime)
            .ThenBy(p => p.Category)
            .ThenBy(p => p.Name)];

        return positions;
    }

    /// <summary>
    /// Gets the base query for positions
    /// </summary>
    /// <param name="include">Flags for what data to include</param>
    /// <returns>IQueryable</returns>
    private IQueryable<Position> GetBaseQuery(IncludePositionProperties include)
    {
        IQueryable<Position> query = context.Positions.AsQueryable();

        if (include.HasFlag(IncludePositionProperties.Location))
        {
            query = query.Include(p => p.Location);
        }

        if (include.HasFlag(IncludePositionProperties.TeamsWithMembers))
        {
            //query = query
            //            .Include(p => p.PositionTeams)
            //                .ThenInclude(pt => pt.Team)
            //                    .ThenInclude(t => t!.Members);
        }
        else if (include.HasFlag(IncludePositionProperties.Teams))
        {
            //query = query
            //            .Include(p => p.PositionTeams)
            //                .ThenInclude(pt => pt.Team);
        }

        return query;
    }
}
