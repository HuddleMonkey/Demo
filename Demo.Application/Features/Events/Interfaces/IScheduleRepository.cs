using Demo.Application.Features.Events.Models;

namespace Demo.Application.Features.Events.Interfaces;

public interface IScheduleRepository : IRepository<Schedule>
{
    /// <summary>
    /// Gets the schedules that reference the positions
    /// </summary>
    /// <param name="positionIds">Ids of the positions</param>
    /// <param name="include">Flags for what data to include</param>
    /// <returns>List of Schedule</returns>
    Task<List<Schedule>> GetSchedulesWithPositionsAsync(List<long> positionIds, IncludeScheduleProperties include = IncludeScheduleProperties.None);
}
