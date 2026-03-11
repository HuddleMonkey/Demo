using Demo.Application.Features.Teams.Interfaces;
using Demo.Application.Features.Teams.Models;
using Demo.Application.Features.Users.Models;

namespace Demo.Application.Features.Teams.Commands;

/// <summary>
/// Sets the team leaders for the list of teams.
/// </summary>
public class SetTeamLeaders
{
    /// <summary>
    /// Command
    /// </summary>
    /// <param name="teams">Teams to set leader for</param>
    public class Command(List<Team> teams) : BaseRequest, IRequest<Result<Empty>>
    {
        /// <summary>
        /// Teams to set leader for
        /// </summary>
        public List<Team> Teams { get; init; } = teams;
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Teams).NotNull();
            RuleFor(x => x.CurrentUser).NotNull();
        }
    }

    public class Handler(ITeamRepository teamRepository, ILogger<Handler> logger) : IRequestHandler<Command, Result<Empty>>
    {
        public async Task<Result<Empty>> Handle(Command request, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Params: # of teams {request.Teams.Count}");

            if (request.Teams.Any())
            {
                // Get all of the team ids in the list
                List<long> teamIds = [.. request.Teams.Flatten(t => t.Children).Select(t => t.Id)];

                // Get the team leaders for team ids
                List<UserClaim> leaders = await teamRepository.GetTeamLeadersAsync(teamIds);
                foreach (var team in request.Teams.Flatten(t => t.Children))
                {
                    team.LeaderUserIds = [.. leaders
                        .Where(l => l.ClaimValue == team.Id.ToString())
                        .Select(l => l.UserId)];
                }
            }

            return Result.Success<Empty>();
        }
    }
}
