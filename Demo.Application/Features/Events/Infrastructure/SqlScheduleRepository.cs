using Demo.Application.Features.Events.Interfaces;
using Demo.Application.Features.Events.Models;

namespace Demo.Application.Features.Events.Infrastructure;

public class SqlScheduleRepository(DemoDbContext context, ILogger<SqlScheduleRepository> repoLogger) : Repository<Schedule>(context, repoLogger), IScheduleRepository
{
    /// <summary>
    /// Gets the schedules that reference the positions
    /// </summary>
    /// <param name="positionIds">Ids of the positions</param>
    /// <param name="include">Flags for what data to include</param>
    /// <returns>List of Schedule</returns>
    public async Task<List<Schedule>> GetSchedulesWithPositionsAsync(List<long> positionIds, IncludeScheduleProperties include = IncludeScheduleProperties.None)
    {
        logger.LogDebug($"Params: # of positionIds={positionIds.Count}, include={include}");

        if (!positionIds.Any()) return [];

        IQueryable<Schedule> query = GetBaseQuery(include);
        List<Schedule> schedules = await query.Where(s => s.PositionId != null && positionIds.Contains(s.PositionId.Value)).ToListAsync();

        return schedules;
    }

    /// <summary>
    /// Gets the base query for schedules
    /// </summary>
    /// <param name="include">Flags for what data to include</param>
    /// <returns>IQueryable</returns>
    private IQueryable<Schedule> GetBaseQuery(IncludeScheduleProperties include)
    {
        IQueryable<Schedule> query = context.Schedules.AsQueryable();

        if (include.HasFlag(IncludeScheduleProperties.User))
        {
            query = query.Include(s => s.User);
        }

        return query;
    }
}
