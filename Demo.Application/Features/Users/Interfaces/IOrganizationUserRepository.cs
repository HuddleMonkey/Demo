using Demo.Application.Features.Users.Models;

namespace Demo.Application.Features.Users.Interfaces;

public interface IOrganizationUserRepository : IRepository<OrganizationUser>
{
    /// <summary>
    /// Get the metadata of the users associated with the organization
    /// </summary>
    /// <param name="organizationId">Id of the organization</param>
    /// <param name="include">Flags for what data to include</param>
    /// <returns>List of OrganizationUser</returns>
    Task<List<OrganizationUser>> GetOrganizationUsersAsync(long organizationId, IncludeOrganizationUserProperties include = IncludeOrganizationUserProperties.None);

    /// <summary>
    /// Gets the metadata for the organization users
    /// </summary>
    /// <param name="organizationId">Id of the organization</param>
    /// <param name="userIds">Ids of the users</param>
    /// <param name="include">Flags for what data to include</param>
    /// <returns>List of OrganizationUser</returns>
    Task<List<OrganizationUser>> GetOrganizationUsersAsync(long organizationId, List<string> userIds, IncludeOrganizationUserProperties include = IncludeOrganizationUserProperties.None);

    /// <summary>
    /// Gets the metadata for the organization user
    /// </summary>
    /// <param name="organizationId">Id of the organization</param>
    /// <param name="userId">Id of the user</param>
    /// <returns>OrganizationUser or NULL if not found</returns>
    Task<OrganizationUser?> GetOrganizationUserAsync(long organizationId, string userId);

    /// <summary>
    /// Get the metadata for the user associated with any organization
    /// </summary>
    /// <param name="userId">Id of the user</param>
    /// <returns>List of OrganizationUser</returns>
    Task<List<OrganizationUser>> GetOrganizationUsersAsync(string userId);
}
