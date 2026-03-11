using Demo.Application.Features.Teams.Models;
using Demo.Application.Features.Users.Models;

namespace Demo.Application.Features.Teams.Interfaces;

public interface ITeamRepository : IRepository<Team>
{
    /// <summary>
    /// Gets the teams in an organization.
    /// </summary>
    /// <param name="organizationId">Id of the organization</param>
    /// <param name="include">What properties to include. Default is none.</param>
    /// <returns>List of Teams</returns>
    Task<List<Team>> GetTeamsInOrganizationAsync(long organizationId, IncludeTeamProperties include = IncludeTeamProperties.None);

    /// <summary>
    /// Get the teams the user is a member of. Includes all members of the team and its parent hierarchy.
    /// </summary>
    /// <param name="userId">User Id</param>
    /// <param name="organizationId">Organization id to filter the teams by</param>
    /// <returns>List of teams the user is a member of</returns>
    Task<List<Team>> GetUserTeamsAsync(string userId, long organizationId);

    /// <summary>
    /// Get the team leader claims for the list of team ids.
    /// </summary>
    /// <param name="teamIds">Team Ids to get the leader claims for</param>
    /// <returns>List of UserClaim</returns>
    Task<List<UserClaim>> GetTeamLeadersAsync(List<long> teamIds);
}
