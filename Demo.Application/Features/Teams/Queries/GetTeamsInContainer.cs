using Demo.Application.Features.Teams.Interfaces;
using Demo.Application.Features.Teams.Models;

namespace Demo.Application.Features.Teams.Queries;

/// <summary>
/// Gets the teams the user has in their "container". If a user is an owner/admin or team leader, they can see all teams
/// in the organization. Otherwise, user can only see teams they are a member of and up the parent hierarchy until they
/// reach a team that is marked as a container or a team that has no parent team (top of the hierarchy).
/// </summary>
public class GetTeamsInContainer
{
    /// <summary>
    /// Query
    /// </summary>
    /// <param name="organizationId">Id of the organization</param>
    /// <param name="include">What properties to include. Default is none.</param>
    public class Query(long organizationId, IncludeTeamProperties include = IncludeTeamProperties.None) : BaseRequest, IRequest<List<Team>>
    {
        /// <summary>
        /// Id of the organization
        /// </summary>
        public long OrganizationId { get; init; } = organizationId;

        /// <summary>
        /// What properties to include. Default is none.
        /// </summary>
        public IncludeTeamProperties Include { get; init; } = include;
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.CurrentUser).NotNull();
        }
    }

    public class Handler(IMediator mediator, ITeamRepository teamRepository, ILogger<Handler> logger) : IRequestHandler<Query, List<Team>>
    {
        public async Task<List<Team>> Handle(Query request, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Params: organizationId={request.OrganizationId}, currentUserId={request.CurrentUser!.GetUserId()}, include={request.Include}");

            // Get the teams in the organization.
            List<Team> teams = await teamRepository.GetTeamsInOrganizationAsync(request.OrganizationId, request.Include);

            // If the user is an owner/admin in the organization, they can see all teams.
            // Otherwise, pair down the list to only teams in the user container
            if (!request.CurrentUser!.IsOwnerOrAdmin(request.OrganizationId))
            {
                // Get a list of teams the user is a member of
                List<Team> members = await mediator.Send(new GetUserTeams.Query(request.CurrentUser!.GetUserId(), request.OrganizationId));

                // For each team, walk up hierarchy until you (a) reach team that is a container or (b) reach a team that has no parent
                // Add that team to the sub list for processing.
                List<Team> container = [];
                foreach (var team in members.Where(t => t.OrganizationId == request.OrganizationId))
                {
                    TraverseParents(team);
                    void TraverseParents(Team team)
                    {
                        if (team.IsContainer || team.Parent is null)
                            container.Add(team);
                        else
                            TraverseParents(team.Parent);
                    }
                }

                teams = [.. container.Flatten(t => t.Children).OrderBy(t => t.Name)];
            }

            return teams;
        }
    }
}
