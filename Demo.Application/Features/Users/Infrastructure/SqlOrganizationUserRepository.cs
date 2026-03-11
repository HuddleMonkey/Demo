using Demo.Application.Features.Users.Interfaces;
using Demo.Application.Features.Users.Models;

namespace Demo.Application.Features.Users.Infrastructure;

public class SqlOrganizationUserRepository(DemoDbContext context, ILogger<SqlOrganizationUserRepository> repoLogger) : Repository<OrganizationUser>(context, repoLogger), IOrganizationUserRepository
{
    /// <summary>
    /// Get the metadata of the users associated with the organization
    /// </summary>
    /// <param name="organizationId">Id of the organization</param>
    /// <param name="include">Flags for what data to include</param>
    /// <returns>List of OrganizationUser</returns>
    public async Task<List<OrganizationUser>> GetOrganizationUsersAsync(long organizationId, IncludeOrganizationUserProperties include = IncludeOrganizationUserProperties.None)
    {
        logger.LogDebug($"Params: organizationId={organizationId}, include={include}");

        IQueryable<OrganizationUser> query = GetBaseQuery(include).Where(o => o.OrganizationId == organizationId);
        List<OrganizationUser> users = await query.ToListAsync();

        return users;
    }

    /// <summary>
    /// Gets the metadata for the organization users
    /// </summary>
    /// <param name="organizationId">Id of the organization</param>
    /// <param name="userIds">Ids of the users</param>
    /// <param name="include">Flags for what data to include</param>
    /// <returns>List of OrganizationUser</returns>
    public async Task<List<OrganizationUser>> GetOrganizationUsersAsync(long organizationId, List<string> userIds, IncludeOrganizationUserProperties include = IncludeOrganizationUserProperties.None)
    {
        logger.LogDebug($"Params: organizationId={organizationId}, # of userIds={userIds.Count}, include={include}");

        IQueryable<OrganizationUser> query = GetBaseQuery(include).Where(o => o.OrganizationId == organizationId && userIds.Any(i => i == o.UserId));
        List<OrganizationUser> users = await query.ToListAsync();

        return users;
    }

    /// <summary>
    /// Gets the metadata for the organization user
    /// </summary>
    /// <param name="organizationId">Id of the organization</param>
    /// <param name="userId">Id of the user</param>
    /// <returns>OrganizationUser or NULL if not found</returns>
    public async Task<OrganizationUser?> GetOrganizationUserAsync(long organizationId, string userId)
    {
        logger.LogDebug($"Params: organizationId={organizationId}, userId={userId}");

        OrganizationUser? user = await context.OrganizationUsers.FirstOrDefaultAsync(u => u.OrganizationId == organizationId && u.UserId == userId);

        return user;
    }

    /// <summary>
    /// Get the metadata for the user associated with any organization
    /// </summary>
    /// <param name="userId">Id of the user</param>
    /// <returns>List of OrganizationUser</returns>
    public async Task<List<OrganizationUser>> GetOrganizationUsersAsync(string userId)
    {
        logger.LogDebug($"Params: userId={userId}");

        List<OrganizationUser> users = await context.OrganizationUsers
            .Where(o => o.UserId == userId)
            .ToListAsync();

        return users;
    }

    /// <summary>
    /// Gets the base query for organization users
    /// </summary>
    /// <param name="include">Flags for what data to include</param>
    /// <returns>IQueryable</returns>
    private IQueryable<OrganizationUser> GetBaseQuery(IncludeOrganizationUserProperties include)
    {
        IQueryable<OrganizationUser> query = context.OrganizationUsers.AsQueryable();

        if (!include.HasFlag(IncludeOrganizationUserProperties.DeletedUsers))
        {
            query = query.Where(o => o.Status != UserStatus.Deleted);
        }

        return query;
    }
}
