using Demo.Application.Features.Teams.Commands;
using Demo.Application.Features.Teams.Interfaces;
using Demo.Application.Features.Teams.Models;

namespace Demo.Application.Features.Teams.Queries;

/// <summary>
/// Gets the teams the user owns. A user owns a team if they are an admin or a team leader. This includes
/// all child teams of the teams they own.
/// </summary>
public class GetTeamsUserOwns
{
    /// <summary>
    /// Query
    /// </summary>
    /// <param name="organizationId">Id of the organization</param>
    /// <param name="flattenTeams">If true, flattens the team list to include all teams in the list, not in hierarchical form. Default is true.</param>
    /// <param name="include">What properties to include. Default is none.</param>
    public class Query(long organizationId, bool flattenTeams = true, IncludeTeamProperties include = IncludeTeamProperties.None) : BaseRequest, IRequest<List<Team>>
    {
        /// <summary>
        /// Id of the organization
        /// </summary>
        public long OrganizationId { get; init; } = organizationId;

        /// <summary>
        /// If true, flattens the team list to include all teams in the list, not in hierarchical form
        /// </summary>
        public bool FlattenTeams { get; init; } = flattenTeams;

        /// <summary>
        /// What properties to include. Default is none.
        /// </summary>
        public IncludeTeamProperties Include { get; init; } = include;
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.OrganizationId).GreaterThan(0);
            RuleFor(x => x.CurrentUser).NotNull();
        }
    }

    public class Handler(IMediator mediator, ITeamRepository teamRepository, ILogger<Handler> logger) : IRequestHandler<Query, List<Team>>
    {
        public async Task<List<Team>> Handle(Query request, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Params: OrganizationId={request.OrganizationId}, CurrentUserId={request.CurrentUser!.GetUserId()}, FlattenTeams={request.FlattenTeams}");

            // Get the teams in the organization.
            List<Team> teams = await teamRepository.GetTeamsInOrganizationAsync(request.OrganizationId, request.Include);

            // If the user is an owner or admin in the organization, they can see all teams. Only return the top level teams (teams with no parents).
            // Otherwise, only return the teams the user is a team leader of.
            if (request.CurrentUser!.IsOwnerOrAdmin(request.OrganizationId))
            {
                teams = [.. teams.Where(t => t.ParentId == 0)];
            }
            else
            {
                // Filter down to every team the user is a leader of
                List<long> leader = request.CurrentUser!.GetTeamsUserLeads();
                teams = [.. teams.Where(t => leader.Any(l => l == t.Id))];

                // We need to weed out any child team that the user is also a team leader of as we only want it to show once, since teams will always show their children
                List<long> toRemove = [];
                teams.ForEach(t => { TraverseChildren(t); });
                toRemove = [.. toRemove.Distinct()];
                teams.RemoveAll(t => toRemove.Any(r => r == t.Id));

                // Inline function to walk the children tree and add any child team whose Id is in the list of leaders
                void TraverseChildren(Team team)
                {
                    foreach (Team child in team.Children)
                    {
                        if (leader.Any(l => l == child.Id)) toRemove.Add(child.Id);
                        TraverseChildren(child);
                    }
                }
            }

            // Include leaders in needed
            if (request.Include.HasFlag(IncludeTeamProperties.Leaders))
            {
                await mediator.Send(new SetTeamLeaders.Command(teams));
            }

            // Flatten list
            if (request.FlattenTeams)
            {
                teams = [.. teams.Flatten(t => t.Children).OrderBy(t => t.Name)];
            }

            return teams;
        }
    }
}
