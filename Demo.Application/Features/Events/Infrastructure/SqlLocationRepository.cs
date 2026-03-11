using Demo.Application.Features.Events.Interfaces;
using Demo.Application.Features.Events.Models;

namespace Demo.Application.Features.Events.Infrastructure;

public class SqlLocationRepository(DemoDbContext context, ILogger<SqlLocationRepository> repoLogger) : Repository<Location>(context, repoLogger), ILocationRepository
{
    /// <summary>
    /// Gets all locations in the organization
    /// </summary>
    /// <param name="organizationId">Id of the organization</param>
    /// <returns>List of Location</returns>
    public async Task<List<Location>> GetLocationsAsync(long organizationId)
    {
        logger.LogDebug($"Params: organizationId={organizationId}");

        List<Location> locations = await context.Locations
            .Where(l => l.OrganizationId == organizationId)
            .OrderBy(l => l.Name)
            .ToListAsync();

        return locations;
    }

    /// <summary>
    /// Gets the location with the given id
    /// </summary>
    /// <param name="id">Id of the location</param>
    /// <returns>Location or NULL if not found</returns>
    public async Task<Location?> GetLocationAsync(long id)
    {
        logger.LogDebug($"Params: id={id}");

        Location? location = await context.Locations.FirstOrDefaultAsync(l => l.Id == id);

        return location;
    }
}
