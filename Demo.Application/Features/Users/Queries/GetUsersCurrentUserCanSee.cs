using Demo.Application.Features.Teams.Models;
using Demo.Application.Features.Teams.Queries;
using Demo.Application.Features.Users.Models;

namespace Demo.Application.Features.Users.Queries;

/// <summary>
/// Gets all users the current user can see in the organization based on team visibility
/// </summary>
public class GetUsersCurrentUserCanSee
{
    /// <summary>
    /// Query
    /// </summary>
    /// <param name="organizationId">Id of the organization</param>
    /// <param name="teamVisibility">Teams visibility</param>
    public class Query(long organizationId, TeamVisibility teamVisibility) : BaseRequest, IRequest<List<AppUser>>
    {
        /// <summary>
        /// Id of the organization
        /// </summary>
        public long OrganizationId { get; init; } = organizationId;

        /// <summary>
        /// Teams visibility
        /// </summary>
        public TeamVisibility TeamVisibility { get; init; } = teamVisibility;
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.OrganizationId).GreaterThan(0);
            RuleFor(x => x.TeamVisibility).IsInEnum();
            RuleFor(x => x.CurrentUser).NotNull();
        }
    }

    public class Handler(IMediator mediator, ILogger<Handler> logger) : IRequestHandler<Query, List<AppUser>>
    {
        public async Task<List<AppUser>> Handle(Query request, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Params: CurrentUserId={request.CurrentUser!.GetUserId()}, OrganizationId={request.OrganizationId}, TeamVisibilityType={request.TeamVisibility}");

            List<AppUser> users = [];

            if (request.CurrentUser!.IsOwnerOrAdmin(request.OrganizationId))
            {
                List<AppUser> organizationUsers = await mediator.Send(new GetUsersInOrganization.Query(request.OrganizationId));
                users.AddRange(organizationUsers);
            }
            else
            {
                List<Team> teams = request.TeamVisibility == TeamVisibility.TeamsUserOwns ?
                    await mediator.Send(new GetTeamsUserOwns.Query(request.OrganizationId, flattenTeams: true, IncludeTeamProperties.Members)) :
                    await mediator.Send(new GetTeamsInContainer.Query(request.OrganizationId, IncludeTeamProperties.Members));

                List<AppUser> organizationUsers = [.. teams.SelectMany(t => t.Members).DistinctBy(u => u.Id).OrderBy(u => u.FullName)];
                foreach (var user in organizationUsers.Where(u => u.OrganizationUser != null))
                {
                    user.OrganizationUser!.OrganizationId = request.OrganizationId;
                }

                users.AddRange(organizationUsers);
            }

            return users;
        }
    }
}
