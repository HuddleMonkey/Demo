using Demo.Shared.Constants;
using System.Security.Claims;

namespace Demo.Shared.Extensions;

/// <summary>
/// Extensions for ClaimsPrincipal
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Gets the user ID from the ClaimsPrincipal
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    /// <returns>User ID or NULL if not found</returns>
    public static string GetUserId(this ClaimsPrincipal user) => user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

    /// <summary>
    /// Gets the user's email from the ClaimsPrincipal
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    /// <returns>User's email or NULL if not found</returns>
    public static string GetEmail(this ClaimsPrincipal user) => user.FindFirst(c => c.Type == ClaimTypes.Name)?.Value ?? string.Empty;

    /// <summary>
    /// Gets the user's first name from the ClaimsPrincipal
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    /// <returns>First name or NULL if not found</returns>
    public static string GetFirstName(this ClaimsPrincipal user) => user.FindFirst(c => c.Type == ClaimTypes.GivenName)?.Value ?? string.Empty;

    /// <summary>
    /// Gets the user's last name from the ClaimsPrincipal
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    /// <returns>Last name or NULL if not found</returns>
    public static string GetLastName(this ClaimsPrincipal user) => user.FindFirst(c => c.Type == ClaimTypes.Surname)?.Value ?? string.Empty;

    /// <summary>
    /// Gets the user's full name from the ClaimsPrincipal
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    /// <returns>Full name</returns>
    public static string GetFullName(this ClaimsPrincipal user) => $"{user.GetFirstName()} {user.GetLastName()}";

    /// <summary>
    /// Gets the user's avatar from the ClaimsPrincipal
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    /// <returns>Avatar or NULL if not found</returns>
    public static string GetAvatar(this ClaimsPrincipal user) => user.FindFirst(c => c.Type == "Avatar")?.Value ?? string.Empty;

    /// <summary>
    /// Returns true if user is a member of an organization
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    /// <returns>True if associated with an organization</returns>
    public static bool IsInAnyOrganization(this ClaimsPrincipal user)
    {
        return user.Claims.Any(c => c.Type == DemoClaimTypes.Organization);
    }

    /// <summary>
    /// Returns true if user is a member of more than one organization
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    /// <returns>True if associated with more than one organization</returns>
    public static bool IsInMoreThanOneOrganization(this ClaimsPrincipal user)
    {
        return user.Claims.Count(c => c.Type == DemoClaimTypes.Organization) > 1;
    }

    /// <summary>
    /// Checks if the user is a member of the organization based on the 'Organization' claim in the format of 'organizationId-organizationName'
    /// </summary>
    /// <param name="user">User</param>
    /// <param name="organizationId">Id of the organization to check</param>
    /// <returns>True if a member of the organization</returns>
    public static bool IsInOrganization(this ClaimsPrincipal user, long organizationId)
    {
        var claim = user.FindFirst(c => c.Type == DemoClaimTypes.Organization && c.Value.StartsWith($"{organizationId}-"));
        return !string.IsNullOrEmpty(claim?.Value);
    }

    /// <summary>
    /// Gets the organization name from the ClaimsPrincipal based on the 'Organization' claim in the format of 'organizationId-organizationName'
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    /// <param name="organizationId">Id of the organization to get the name for</param>
    /// <returns>Name of the organization</returns>
    public static string GetOrganizationName(this ClaimsPrincipal user, long organizationId)
    {
        var claim = user.FindFirst(c => c.Type == DemoClaimTypes.Organization && c.Value.StartsWith($"{organizationId}-"));
        if (!string.IsNullOrEmpty(claim?.Value))
        {
            string[] parts = claim.Value.Split('-');
            if (parts.Length == 2)
            {
                return parts[1];
            }
        }

        return "";
    }

    /// <summary>
    /// Get a list of all the teams the user is a leader of
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    /// <returns>List of team ids the user is a leader of</returns>
    public static List<long> GetTeamsUserLeads(this ClaimsPrincipal user) => [.. user.Claims.Where(c => c.Type == DemoClaimTypes.TeamLeader).Select(c => Convert.ToInt64(c.Value))];

    /// <summary>
    /// Checks if the user is a team leader
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    /// <returns>True if team leader</returns>
    public static bool IsTeamLeader(this ClaimsPrincipal user) => user.HasClaim(c => c.Type == DemoClaimTypes.TeamLeader);

    /// <summary>
    /// Checks if the user is a team leader for the specified team
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    /// <param name="teamId">Id of the team to check</param>
    /// <returns>True if team leader of the specified team</returns>
    public static bool IsTeamLeader(this ClaimsPrincipal user, long teamId) => user.HasClaim(c => c.Type == DemoClaimTypes.TeamLeader && c.Value == teamId.ToString());

    /// <summary>
    /// Checks if the user is a team leader or above
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    /// <returns>True if team leader or above</returns>
    public static bool IsTeamLeaderOrAbove(this ClaimsPrincipal user) => user.IsTeamLeader() || user.IsOwnerOrAdmin();

    /// <summary>
    /// Checks if the current user is the owner or admin in the organization.
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    /// <returns>True if owner or admin</returns>
    public static bool IsOwnerOrAdmin(this ClaimsPrincipal user)
    {
        return user.HasClaim(c =>
            c.Type == DemoClaimTypes.OrganizationOwner ||
            c.Type == DemoClaimTypes.OrganizationAdmin);
    }

    /// <summary>
    /// Checks if the current user is the owner or admin in the organization.
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    /// <param name="organizationId">Id of the organization to check</param>
    /// <returns>True if owner or admin</returns>
    public static bool IsOwnerOrAdmin(this ClaimsPrincipal user, long organizationId)
    {
        return user.HasClaim(c =>
            (c.Type == DemoClaimTypes.OrganizationOwner && c.Value == organizationId.ToString()) ||
            (c.Type == DemoClaimTypes.OrganizationAdmin && c.Value == organizationId.ToString()));
    }

    /// <summary>
    /// Get a list of all the organizations where user is admin or owner
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    /// <returns>List of organization ids the user is admin or owner</returns>
    public static List<long> GetOrganizationsUserIsAdminOrOwner(this ClaimsPrincipal user) => [.. user.Claims.Where(c => c.Type == DemoClaimTypes.OrganizationOwner || c.Type == DemoClaimTypes.OrganizationAdmin).Select(c => Convert.ToInt64(c.Value))];

    /// <summary>
    /// Get a list of all the organizations the user owns
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    /// <returns>List of organization ids the user owns</returns>
    public static List<long> GetOrganizationsUserOwns(this ClaimsPrincipal user) => [.. user.Claims.Where(c => c.Type == DemoClaimTypes.OrganizationOwner).Select(c => Convert.ToInt64(c.Value))];

    /// <summary>
    /// Get a list of all the organizations the user is a member of
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    /// <returns>List of organization ids the user is a member of</returns>
    public static List<long> GetOrganizationsUserIsMemberOf(this ClaimsPrincipal user)
    {
        List<long> organizationIds = [];

        List<string> organizations = [.. user.Claims.Where(c => c.Type == DemoClaimTypes.Organization).Select(c => c.Value)];
        foreach (var organization in organizations)
        {
            string[] parts = organization.Split('-');
            if (parts.Length == 2)
            {
                organizationIds.Add(Convert.ToInt64(parts[0]));
            }
        }

        return organizationIds;
    }

    /// <summary>
    /// Checks if the current user is an admin in the organization.
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    /// <param name="organizationId">Id of the organization to check</param>
    /// <returns>True if admin</returns>
    public static bool IsAdmin(this ClaimsPrincipal user, long organizationId)
    {
        return user.HasClaim(c => c.Type == DemoClaimTypes.OrganizationAdmin && c.Value == organizationId.ToString());
    }

    /// <summary>
    /// Checks if the current user is the owner in the organization.
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    /// <returns>True if owner</returns>
    public static bool IsOwner(this ClaimsPrincipal user)
    {
        return user.HasClaim(c => c.Type == DemoClaimTypes.OrganizationOwner);
    }

    /// <summary>
    /// Checks if the current user is the owner in the organization.
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    /// <param name="organizationId">Id of the organization to check</param>
    /// <returns>True if owner or admin</returns>
    public static bool IsOwner(this ClaimsPrincipal user, long organizationId)
    {
        return user.HasClaim(c => c.Type == DemoClaimTypes.OrganizationOwner && c.Value == organizationId.ToString());
    }

    /// <summary>
    /// Checks if the current user is a provider
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    /// <returns>True if provider</returns>
    public static bool IsProvider(this ClaimsPrincipal user)
    {
        return user.HasClaim(c => c.Type == DemoClaimTypes.Provider);
    }

    /// <summary>
    /// Checks if the current user is a provider
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    /// <param name="providerId">Id of the provider to check</param>
    /// <returns>True if provider</returns>
    public static bool IsProvider(this ClaimsPrincipal user, long providerId)
    {
        return user.HasClaim(c => c.Type == DemoClaimTypes.Provider && c.Value == providerId.ToString());
    }

    /// <summary>
    /// Get a list of all the providers the user is a member of
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    /// <returns>List of provider ids the user is a member of</returns>
    public static List<long> GetProvidersUserIsMember(this ClaimsPrincipal user) => [.. user.Claims.Where(c => c.Type == DemoClaimTypes.Provider).Select(c => Convert.ToInt64(c.Value))];

    /// <summary>
    /// Checks if the current user is a Huddle Monkey Admin
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    /// <returns>True if admin</returns>
    public static bool IsAdmin(this ClaimsPrincipal user)
    {
        return user.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == DemoRoles.Admin);
    }
}
