using Demo.Application.Features.Organizations.Models;

namespace Demo.Application.Features.Organizations.Interfaces;

public interface IOrganizationRepository : IRepository<Organization>
{
    /// <summary>
    /// Gets organizations by Ids
    /// </summary>
    /// <param name="ids">Organization ids to retrieve</param>
    /// <param name="include">What properties to include</param>
    /// <returns>List of Organizations</returns>
    Task<List<Organization>> GetOrganizationsAsync(List<long> ids, IncludeOrganizationProperties include = IncludeOrganizationProperties.None);

    /// <summary>
    /// Gets an organization by ID.
    /// </summary>
    /// <param name="id">ID of the organization to retrieve</param>
    /// <param name="include">What properties to include</param>
    /// <returns>Organization, or NULL if not found</returns>
    Task<Organization?> GetOrganizationAsync(long id, IncludeOrganizationProperties include = IncludeOrganizationProperties.None);
}
