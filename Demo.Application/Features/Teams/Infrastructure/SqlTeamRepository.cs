using Demo.Application.Features.Teams.Interfaces;
using Demo.Application.Features.Teams.Models;
using Demo.Application.Features.Users.Models;

namespace Demo.Application.Features.Teams.Infrastructure;

public class SqlTeamRepository(DemoDbContext context, ILogger<SqlTeamRepository> repoLogger) : Repository<Team>(context, repoLogger), ITeamRepository
{
    /// <summary>
    /// Gets the teams in an organization.
    /// </summary>
    /// <param name="organizationId">Id of the organization</param>
    /// <param name="include">What properties to include. Default is none.</param>
    /// <returns>List of Teams</returns>
    public async Task<List<Team>> GetTeamsInOrganizationAsync(long organizationId, IncludeTeamProperties include = IncludeTeamProperties.None)
    {
        logger.LogDebug($"Params: organizationId={organizationId}, include={include}");

        IQueryable<Team> query = GetBaseQuery(include).Where(t => t.OrganizationId == organizationId).OrderBy(t => t.Name);
        List<Team> teams = await query.ToListAsync();
        CleanUpTeamsPayload(teams);

        foreach (var team in teams)
        {
            team.Parent = teams.FirstOrDefault(t => t.Id == team.ParentId);
            team.Children = [.. teams.Where(t => t.ParentId.HasValue && t.ParentId == team.Id)];
        }

        return teams;
    }

    /// <summary>
    /// Get the teams the user is a member of. Includes all members of the team and its parent hierarchy.
    /// </summary>
    /// <param name="userId">User Id</param>
    /// <param name="organizationId">Organization id to filter the teams by</param>
    /// <returns>List of teams the user is a member of</returns>
    public async Task<List<Team>> GetUserTeamsAsync(string userId, long organizationId)
    {
        logger.LogDebug($"Params: userId={userId}, organizationId={organizationId}");

        // Get the teams the user is a member of
        IQueryable<Team> query = context.Teams.AsQueryable()
            .Include(t => t.Members.OrderBy(u => u.LastName).ThenBy(u => u.FirstName))
            .Where(t => t.Members.Any(u => u.Id == userId))
            .OrderBy(t => t.Name);

        if (organizationId > 0)
        {
            query = query.Where(t => t.OrganizationId == organizationId);
        }

        List<Team> teams = await query.ToListAsync();
        CleanUpTeamsPayload(teams);

        // Get all teams in the organizations
        List<Team> allTeamsInOrganization = [];
        List<long> organizationIds = [.. teams.Select(t => t.OrganizationId).Distinct()];
        foreach (var oid in organizationIds)
        {
            List<Team> teamsInOrganization = await GetTeamsInOrganizationAsync(oid);
            allTeamsInOrganization.AddRange(teamsInOrganization);
        }

        // Connect the parent and children
        foreach (var team in teams)
        {
            team.Parent = allTeamsInOrganization.FirstOrDefault(t => t.Id == team.ParentId);
            team.Children = [.. allTeamsInOrganization.Where(t => t.ParentId.HasValue && t.ParentId == team.Id)];
        }

        return teams;
    }

    /// <summary>
    /// Get the team leader claims for the list of team ids.
    /// </summary>
    /// <param name="teamIds">Team Ids to get the leader claims for</param>
    /// <returns>List of UserClaim</returns>
    public async Task<List<UserClaim>> GetTeamLeadersAsync(List<long> teamIds)
    {
        logger.LogDebug($"Params: teamIds=total of {teamIds.Count}");
        if (!teamIds.Any()) return [];

        var teamInClause = string.Join(", ", teamIds);
        var sql = $"select * from AspNetUserClaims where ClaimType = '{DemoClaimTypes.TeamLeader}' and ClaimValue in ({teamInClause})";
        var claims = await context.UserClaimsTable
            .FromSqlRaw(sql)
            .ToListAsync();

        return claims;
    }

    /// <summary>
    /// Gets the base query for a team
    /// </summary>
    /// <param name="include">Indicates what data to include</param>
    /// <returns>IQueryable</returns>
    private IQueryable<Team> GetBaseQuery(IncludeTeamProperties include)
    {
        IQueryable<Team> query = context.Teams.AsQueryable();

        if (include.HasFlag(IncludeTeamProperties.Members))
        {
            query = query.Include(t => t.Members.OrderBy(u => u.LastName).ThenBy(u => u.FirstName));
        }

        return query;
    }

    /// <summary>
    /// Cleans up the teams model to NULL out any data to not include in the payload. This is to help
    /// prevent cyclical references and reduce the payload.
    /// </summary>
    /// <param name="teams">List of teams to clean up</param>
    private void CleanUpTeamsPayload(List<Team> teams)
    {
        if (teams is null) return;
        teams.ForEach(CleanUpTeamPayload);
    }

    /// <summary>
    /// Cleans up the teams model to NULL out any data to not include in the payload. This is to help
    /// prevent cyclical references and reduce the payload.
    /// </summary>
    /// <param name="team">Team to clean up</param>
    private void CleanUpTeamPayload(Team? team)
    {
        if (team is null) return;

        // Clear out the teams associated with the members of the team
        team.Members?.ForEach(u => u.Teams = []);

        // Traverse the hierarchy to clean up the payload
        TraverseParents(team);
        TraverseChildren(team);

        void TraverseParents(Team team)
        {
            if (team.Parent is null) return;

            team.Parent.Members?.ForEach(u => u.Teams = []);

            TraverseParents(team.Parent);
            TraverseChildren(team.Parent);
        }

        void TraverseChildren(Team team)
        {
            if (team.Children is null) return;

            foreach (Team child in team.Children)
            {
                child.Members?.ForEach(u => u.Teams = []);
                TraverseChildren(child);
            }
        }
    }
}
