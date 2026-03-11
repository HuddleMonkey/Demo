using Demo.Application.Features.Events.Interfaces;
using Demo.Application.Features.Events.Models;
using Demo.Application.Features.Teams.Models;
using Demo.Application.Features.Teams.Queries;

namespace Demo.Application.Features.Events.Queries;

/// <summary>
/// Gets the event ids the user created or has permission to access or has any of the teams the user leads scheduled
/// </summary>
public class GetEventIdsUserHasPermissions
{
    /// <summary>
    /// Query
    /// </summary>
    /// <param name="organizationId">Id of the associated organization</param>
    /// <param name="userId">Id of the user to get events for</param>
    public class Query(long organizationId, string userId) : BaseRequest, IRequest<Dictionary<long, EventPermissionLevel>>
    {
        /// <summary>
        /// Id of the associated organization
        /// </summary>
        public long OrganizationId { get; init; } = organizationId;

        /// <summary>
        /// Id of the user to get events for
        /// </summary>
        public string UserId { get; init; } = userId;
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.OrganizationId).GreaterThan(0);
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.CurrentUser).NotNull();
        }
    }

    public class Handler(IMediator mediator, IEventRepository eventRepository, /*IPositionTeamRepository positionTeamRepository, ISeriesRepository seriesRepository, ISeriesPartRepository seriesPartRepository, IScheduleRepository scheduleRepository,*/ ILogger<Handler> logger) : IRequestHandler<Query, Dictionary<long, EventPermissionLevel>>
    {
        public async Task<Dictionary<long, EventPermissionLevel>> Handle(Query request, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Params: OrganizationId={request.OrganizationId}, userId={request.UserId}");

            Dictionary<long, EventPermissionLevel> events = [];

            // Get events user created
            List<Event> created = await eventRepository.GetEventsUserCreatedAsync(request.OrganizationId, request.UserId);
            events.AddRange([.. created.Select(e => e.Id)], EventPermissionLevel.Creator);

            // Get events user has permission to see
            //List<EventPermission> permissions = await mediator.Send(new GetEventPermissionsForUser.Query(request.OrganizationId, request.UserId));
            //events.AddRange([.. permissions.Select(e => e.EventId)], EventPermissionLevel.AssignedPermission);

            // Get any events where any of the teams the user leads are scheduled
            List<Team> teams = await mediator.Send(new GetTeamsUserLeads.Query(request.OrganizationId, includeChildTeams: true));
            List<long> teamIds = [.. teams.Select(t => t.Id)];

            if (teamIds.Any())
            {
                //List<PositionTeam> positions = await positionTeamRepository.GetPositionsAssociatedWithTeamsAsync(teamIds);
                //List<long> positionIds = [.. positions.Select(t => t.PositionId).Distinct()];

                //if (positionIds.Any())
                //{
                //    List<Schedule> schedules = await scheduleRepository.GetSchedulesWithPositionsAsync(positionIds);
                //    List<long> seriesPartIds = [.. schedules.Select(s => s.SeriesPartId).Distinct()];

                //    if (seriesPartIds.Any())
                //    {
                //        List<SeriesPart> seriesParts = await seriesPartRepository.GetSeriesPartsAsync(seriesPartIds);
                //        List<long> seriesIds = [.. seriesParts.Select(p => p.SeriesId).Distinct()];

                //        if (seriesIds.Any())
                //        {
                //            List<Series> series = await seriesRepository.GetSeriesAsync(seriesIds);
                //            events.AddRange([.. series.Select(s => s.EventId).Distinct()], EventPermissionLevel.TeamLeader);
                //        }
                //    }
                //}
            }

            return events;
        }
    }
}
