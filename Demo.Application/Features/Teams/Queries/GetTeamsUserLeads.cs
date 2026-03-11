using Demo.Application.Features.Teams.Models;

namespace Demo.Application.Features.Teams.Queries;

/// <summary>
/// Gets the teams the user is a team leader of.
/// </summary>
public class GetTeamsUserLeads
{
    /// <summary>
    /// Query
    /// </summary>
    /// <param name="organizationId">Id of the organization</param>
    /// <param name="includeChildTeams">If true, include child teams of the teams the user leads</param>
    /// <param name="include">What properties to include. Default is none.</param>
    public class Query(long organizationId, bool includeChildTeams, IncludeTeamProperties include = IncludeTeamProperties.None) : BaseRequest, IRequest<List<Team>>
    {
        /// <summary>
        /// Id of the organization
        /// </summary>
        public long OrganizationId { get; init; } = organizationId;

        /// <summary>
        /// If true, include child teams of the teams the user leads
        /// </summary>
        public bool IncludeChildTeams { get; init; } = includeChildTeams;

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

    public class Handler(IMediator mediator, ILogger<Handler> logger) : IRequestHandler<Query, List<Team>>
    {
        public async Task<List<Team>> Handle(Query request, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Params: OrganizationId={request.OrganizationId}, User={request.CurrentUser!.GetUserId()}, includeChildTeams={request.IncludeChildTeams}");

            List<long> leaderIds = request.CurrentUser!.GetTeamsUserLeads();
            List<Team> teams = await mediator.Send(new GetTeamsUserOwns.Query(request.OrganizationId, flattenTeams: true, request.Include));
            teams = [.. teams.Where(t => leaderIds.Any(l => l == t.Id))];

            if (request.IncludeChildTeams)
            {
                teams = teams.Flatten(t => t.Children);
            }

            return teams;
        }
    }
}
