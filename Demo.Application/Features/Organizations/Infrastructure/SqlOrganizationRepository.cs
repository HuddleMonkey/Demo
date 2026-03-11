using Demo.Application.Features.Organizations.Interfaces;
using Demo.Application.Features.Organizations.Models;

namespace Demo.Application.Features.Organizations.Infrastructure;

public class SqlOrganizationRepository(DemoDbContext context, ILogger<SqlOrganizationRepository> repoLogger) : Repository<Organization>(context, repoLogger), IOrganizationRepository
{
    /// <summary>
    /// Gets organizations by Ids
    /// </summary>
    /// <param name="ids">Organization ids to retrieve</param>
    /// <param name="include">What properties to include</param>
    /// <returns>List of Organizations</returns>
    public async Task<List<Organization>> GetOrganizationsAsync(List<long> ids, IncludeOrganizationProperties include = IncludeOrganizationProperties.None)
    {
        logger.LogDebug($"Params: # of organizations={ids.Count}, include={include}");

        IQueryable<Organization> query = GetBaseQuery(include, includeDeleted: true)
            .Where(o => ids.Any(i => i == o.Id))
            .OrderBy(o => o.Name);

        List<Organization> organizations = await query.ToListAsync();

        return organizations;
    }

    /// <summary>
    /// Gets an organization by ID.
    /// </summary>
    /// <param name="id">ID of the organization to retrieve</param>
    /// <param name="include">What properties to include</param>
    /// <returns>Organization, or NULL if not found</returns>
    public async Task<Organization?> GetOrganizationAsync(long id, IncludeOrganizationProperties include = IncludeOrganizationProperties.None)
    {
        logger.LogDebug($"Params: id={id}, include={include}");

        IQueryable<Organization> query = GetBaseQuery(include);
        Organization? organization = await query.SingleOrDefaultAsync(o => o.Id == id);

        return organization;
    }

    /// <summary>
    /// Gets the base query for organizations
    /// </summary>
    /// <param name="include">Flags for what data to include</param>
    /// <param name="includeDeleted">Whether or not to include deleted organizations</param>
    /// <returns>IQueryable</returns>
    private IQueryable<Organization> GetBaseQuery(IncludeOrganizationProperties include, bool includeDeleted = false)
    {
        IQueryable<Organization> query = context.Organizations.AsQueryable();

        if (!includeDeleted)
        {
            query = query.Where(o => o.Status != OrganizationStatus.Deleted);
        }

        if (include.HasFlag(IncludeOrganizationProperties.Industry))
            query = query.Include(o => o.Industry);

        if (include.HasFlag(IncludeOrganizationProperties.Owner))
            query = query.Include(o => o.Owner);

        return query;
    }
}
