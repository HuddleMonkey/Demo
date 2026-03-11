using Demo.Application.Features.Events.Interfaces;
using Demo.Application.Features.Events.Models;
using Demo.Application.Features.Organizations.Models;
using Demo.Application.Features.Organizations.Queries;
using Demo.Application.Features.Teams.Models;
using Demo.Application.Features.Teams.Queries;
using Demo.Application.Features.Users.Models;
using Demo.Application.Features.Users.Queries;

namespace Demo.Application.Features.Events.Queries;

/// <summary>
/// Get the event view
/// </summary>
public class GetEventView
{
    /// <summary>
    /// Query
    /// </summary>
    /// <param name="eventId">Id of the event</param>
    public class Query(long eventId) : BaseRequest, IRequest<Result<EventView>>
    {
        /// <summary>
        /// Id of the event
        /// </summary>
        public long EventId { get; init; } = eventId;
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.EventId).GreaterThan(0);
            RuleFor(x => x.CurrentUser).NotNull();
        }
    }

    public class Handler(IMediator mediator, IEventRepository eventRepository, IPositionRepository positionRepository, /*IPositionTeamRepository positionTeamRepository,*/ ILocationRepository locationRepository, ILogger<Handler> logger) : IRequestHandler<Query, Result<EventView>>
    {
        public async Task<Result<EventView>> Handle(Query request, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Params: EventId={request.EventId}");
            string currentUserId = request.CurrentUser!.GetUserId();

            // Get the event
            Event? @event = await eventRepository.GetEventAsync(request.EventId, IncludeEventProperties.SeriesPartsWithSchedules | IncludeEventProperties.ScheduleTemplates);
            if (@event is null) return Result.Failed<EventView>("Event not found");

            // Check if the user is associated with the event
            Dictionary<long, EventPermissionLevel> eventIds = await mediator.Send(new GetEventIdsUserHasPermissions.Query(@event.OrganizationId, currentUserId));
            if (!eventIds.TryGetValue(request.EventId, out EventPermissionLevel permissionLevel)) return Result.Failed<EventView>("You do not have access to this event");

            // Get the positions
            @event.Positions = await positionRepository.GetPositionsAsync(@event.Id, IncludePositionProperties.Location | IncludePositionProperties.Teams);

            // If the user is not the creator or assigned permissions...
            if (permissionLevel == EventPermissionLevel.TeamLeader)
            {
                // Filter out the series that do not have any of the users teams scheduled
                List<Team> teamsUserLeads = await mediator.Send(new GetTeamsUserLeads.Query(@event.OrganizationId, includeChildTeams: true));
                List<long> teamIds = [.. teamsUserLeads.Select(t => t.Id)];

                if (teamIds.Any())
                {
                    //List<PositionTeam> positions = await positionTeamRepository.GetPositionsAssociatedWithTeamsAsync(teamIds);
                    //List<long> positionIds = [.. positions.Select(t => t.PositionId).Distinct()];

                    //if (positionIds.Any())
                    //{
                    //    @event.Series = [.. @event.Series
                    //        .Where(s => s.Parts.Any(p => p.Schedules.Any(s => s.PositionId != null && positionIds.Contains(s.PositionId.Value))))
                    //        .OrderByDescending(s => s.EndDate)
                    //        .ThenByDescending(s => s.StartDate)];
                    //}
                }

                // Filter out the positions they do not lead
                //@event.Positions = [.. @event.Positions.Where(p => p.Teams.Any(t => teamIds.Contains(t.Id)))];
            }

            // Set the series availability
            TimeZoneSettings settings = await mediator.Send(new GetTimeZoneSettings.Query(@event.OrganizationId));
            foreach (var series in @event.Series)
            {
                series.Availability = DateUtils.GetAvailability(series.StartDate, series.EndDate, settings.Today);
            }

            // Set permissions
            //@event.Permissions = await mediator.Send(new GetEventPermissions.Query(@event.Id, @event.Positions));
            @event.PermissionLevel = permissionLevel;
            @event.ScheduleTemplates.ForEach(t => t.CurrentUserCanEdit = t.CreatedByUserId == currentUserId || permissionLevel != EventPermissionLevel.TeamLeader);

            // Get related data
            List<AppUser> users = await mediator.Send(new GetUsersCurrentUserCanSee.Query(@event.OrganizationId, TeamVisibility.TeamsUserOwns));
            List<Team> teams = await mediator.Send(new GetTeamsUserOwns.Query(@event.OrganizationId));
            List<Location> locations = await locationRepository.GetLocationsAsync(@event.OrganizationId);

            // Build the view
            EventView view = new()
            {
                Event = @event,
                AvailableUsersForPermissions = users,
                Teams = teams,
                Locations = locations
            };

            return Result.Success(view);
        }
    }
}
