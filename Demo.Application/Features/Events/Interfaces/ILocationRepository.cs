using Demo.Application.Features.Events.Models;

namespace Demo.Application.Features.Events.Interfaces;

public interface ILocationRepository : IRepository<Location>
{
    /// <summary>
    /// Gets all locations in the organization
    /// </summary>
    /// <param name="organizationId">Id of the organization</param>
    /// <returns>List of Location</returns>
    Task<List<Location>> GetLocationsAsync(long organizationId);

    /// <summary>
    /// Gets the location with the given id
    /// </summary>
    /// <param name="id">Id of the location</param>
    /// <returns>Location or NULL if not found</returns>
    Task<Location?> GetLocationAsync(long id);
}
