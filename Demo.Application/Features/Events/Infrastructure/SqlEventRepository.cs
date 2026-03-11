using Demo.Application.Features.Events.Interfaces;
using Demo.Application.Features.Events.Models;

namespace Demo.Application.Features.Events.Infrastructure;

public class SqlEventRepository(DemoDbContext context, ILogger<SqlEventRepository> repoLogger) : Repository<Event>(context, repoLogger), IEventRepository
{
    /// <summary>
    /// Get the events with the given ids
    /// </summary>
    /// <param name="ids">Ids of the events to retrieve</param>
    /// <param name="include">Properties to include</param>
    /// <returns>List of events</returns>
    public async Task<List<Event>> GetEventsAsync(List<long> ids, IncludeEventProperties include = IncludeEventProperties.None)
    {
        logger.LogDebug($"Params: # of ids={ids.Count}, include={include}");
        if (!ids.Any()) return [];

        IQueryable<Event> query = GetBaseQuery(include)
            .Where(e => ids.Contains(e.Id))
            .OrderBy(e => e.Name);
        List<Event> events = await query.AsSplitQuery().ToListAsync();

        return events;
    }

    /// <summary>
    /// Gets events the user created
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="include">Flags for what data to include. Default is None.</param>
    /// <returns>List of events</returns>
    public async Task<List<Event>> GetEventsUserCreatedAsync(string userId, IncludeEventProperties include = IncludeEventProperties.None)
    {
        logger.LogDebug($"Params: userId={userId}, include={include}");

        IQueryable<Event> query = GetBaseQuery(include).Where(e => e.CreatedByUserId == userId);
        List<Event> events = await query.AsSplitQuery().ToListAsync();

        return events;
    }

    /// <summary>
    /// Gets events the user created
    /// </summary>
    /// <param name="organizationId">Id of the organization</param>
    /// <param name="userId">User ID</param>
    /// <param name="include">Flags for what data to include. Default is None.</param>
    /// <returns>List of events</returns>
    public async Task<List<Event>> GetEventsUserCreatedAsync(long organizationId, string userId, IncludeEventProperties include = IncludeEventProperties.None)
    {
        logger.LogDebug($"Params: organizationId={organizationId}, userId={userId}, include={include}");

        IQueryable<Event> query = GetBaseQuery(include).Where(e => e.OrganizationId == organizationId && e.CreatedByUserId == userId);
        List<Event> events = await query.AsSplitQuery().ToListAsync();

        return events;
    }

    /// <summary>
    /// Gets an event with the given id
    /// </summary>
    /// <param name="id">Id of the event</param>
    /// <param name="include">Flags for what data to include. Default is None.</param>
    /// <returns>Event or NULL if not found</returns>
    public async Task<Event?> GetEventAsync(long id, IncludeEventProperties include = IncludeEventProperties.None)
    {
        logger.LogDebug($"Params: id={id}, include={include}");

        IQueryable<Event> query = GetBaseQuery(include);
        Event? evt = await query.AsSplitQuery().FirstOrDefaultAsync(e => e.Id == id);

        return evt;
    }

    /// <summary>
    /// Gets the base query for events
    /// </summary>
    /// <param name="include">Flags for what data to include</param>
    /// <returns>IQueryable</returns>
    private IQueryable<Event> GetBaseQuery(IncludeEventProperties include)
    {
        IQueryable<Event> query = context.Events.AsQueryable();

        if (include.HasFlag(IncludeEventProperties.Creator))
        {
            query = query.Include(p => p.CreatedBy);
            query = query.Include(p => p.ModifiedBy);
        }

        if (include.HasFlag(IncludeEventProperties.PositionsDetails))
        {
            query = query
                        .Include(e => e.Positions)
                            .ThenInclude(p => p.Location);
                        //.Include(e => e.Positions)
                            //.ThenInclude(p => p.PositionTeams);
        }
        else if (include.HasFlag(IncludeEventProperties.Positions))
        {
            query = query.Include(e => e.Positions);
        }

        if (include.HasFlag(IncludeEventProperties.SeriesAllDetails))
        {
            query = query
                        //.Include(e => e.Series)
                            //.ThenInclude(s => s.SeriesTags)
                        //.Include(e => e.Series)
                            //.ThenInclude(s => s.Content)
                        .Include(e => e.Series.OrderByDescending(s => s.EndDate).ThenByDescending(s => s.StartDate))
                            .ThenInclude(s => s.Parts.OrderBy(p => p.Date))
                                .ThenInclude(p => p.Schedules);
        }
        else if (include.HasFlag(IncludeEventProperties.SeriesPartsWithSchedulesAndUser))
        {
            query = query
                        //.Include(e => e.Series)
                            //.ThenInclude(s => s.SeriesTags)
                        .Include(e => e.Series.OrderByDescending(s => s.EndDate).ThenByDescending(s => s.StartDate))
                            .ThenInclude(s => s.Parts.OrderBy(p => p.Date))
                                .ThenInclude(p => p.Schedules)
                                    .ThenInclude(s => s.User);
        }
        else if (include.HasFlag(IncludeEventProperties.SeriesPartsWithSchedules))
        {
            query = query
                        //.Include(e => e.Series)
                            //.ThenInclude(s => s.SeriesTags)
                        .Include(e => e.Series.OrderByDescending(s => s.EndDate).ThenByDescending(s => s.StartDate))
                            .ThenInclude(s => s.Parts.OrderBy(p => p.Date))
                                .ThenInclude(p => p.Schedules);
        }
        else if (include.HasFlag(IncludeEventProperties.SeriesParts))
        {
            query = query
                        //.Include(e => e.Series)
                            //.ThenInclude(s => s.SeriesTags)
                        .Include(e => e.Series.OrderByDescending(s => s.EndDate).ThenByDescending(s => s.StartDate))
                            .ThenInclude(s => s.Parts.OrderBy(p => p.Date));
        }
        else if (include.HasFlag(IncludeEventProperties.Series))
        {
            query = query
                        .Include(e => e.Series.OrderByDescending(s => s.EndDate).ThenByDescending(s => s.StartDate));
                            //.ThenInclude(s => s.SeriesTags);
        }

        if (include.HasFlag(IncludeEventProperties.ScheduleTemplatesWithPositionsAndUsers))
        {
            query = query
                        .Include(e => e.ScheduleTemplates.OrderBy(st => st.Name))
                            .ThenInclude(t => t.TemplatePositions)
                                .ThenInclude(p => p.User);
        }
        else if (include.HasFlag(IncludeEventProperties.ScheduleTemplatesWithPositions))
        {
            query = query
                        .Include(e => e.ScheduleTemplates.OrderBy(st => st.Name))
                            .ThenInclude(t => t.TemplatePositions);
        }
        else if (include.HasFlag(IncludeEventProperties.ScheduleTemplates))
        {
            query = query.Include(e => e.ScheduleTemplates.OrderBy(st => st.Name));
        }

        return query;
    }
}
