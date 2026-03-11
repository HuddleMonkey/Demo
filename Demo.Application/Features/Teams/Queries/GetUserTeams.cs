using Demo.Application.Features.Teams.Commands;
using Demo.Application.Features.Teams.Interfaces;
using Demo.Application.Features.Teams.Models;

namespace Demo.Application.Features.Teams.Queries;

/// <summary>
/// Get the teams the user is a member of. Includes all members of the team.
/// </summary>
public class GetUserTeams
{
    /// <summary>
    /// Query
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="organizationId">Optional organization id to filter the user teams by</param>
    /// <param name="include">What properties to include. Default is none.</param>
    public class Query(string userId, long organizationId, IncludeTeamProperties include = IncludeTeamProperties.None) : BaseRequest, IRequest<List<Team>>
    {
        /// <summary>
        /// User ID
        /// </summary>
        public string UserId { get; init; } = userId;

        /// <summary>
        /// Optional organization id to filter the user teams by
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
            RuleFor(x => x.UserId).NotEmpty();
        }
    }

    public class Handler(IMediator mediator, ITeamRepository teamRepository, ILogger<Handler> logger) : IRequestHandler<Query, List<Team>>
    {
        public async Task<List<Team>> Handle(Query request, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Params: userId={request.UserId}, OrganizationId={request.OrganizationId}");

            List<Team> teams = await teamRepository.GetUserTeamsAsync(request.UserId, request.OrganizationId);

            if (request.Include.HasFlag(IncludeTeamProperties.Leaders))
            {
                await mediator.Send(new SetTeamLeaders.Command(teams));
            }

            return teams;
        }
    }
}
