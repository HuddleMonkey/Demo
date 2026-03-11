using Demo.Application.Features.Events.Interfaces;
using Demo.Application.Features.Events.Models;
using Demo.Application.Features.Teams.Models;
using Demo.Application.Features.Teams.Queries;
using Demo.Application.Features.Users.Models;
using Demo.Application.Features.Users.Queries;

namespace Demo.Application.Features.Events.Queries;

/// <summary>
/// Gets the events view with all the events the user has permission to see
/// </summary>
public class GetEventsView
{
    /// <summary>
    /// Query
    /// </summary>
    /// <param name="organizationId">Id of the associated organization to get events for</param>
    /// <param name="userId">Id of the user to get events for</param>
    public class Query(long organizationId, string userId) : BaseRequest, IRequest<Result<EventsView>>
    {
        /// <summary>
        /// Id of the associated organization to get events for
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

    public class Handler(IMediator mediator, IEventRepository eventRepository, ILocationRepository locationRepository, ILogger<Handler> logger) : IRequestHandler<Query, Result<EventsView>>
    {
        public async Task<Result<EventsView>> Handle(Query request, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Params: OrganizationId={request.OrganizationId}, userId={request.UserId}");

            // Get the user's events
            Dictionary<long, EventPermissionLevel> eventIds = await mediator.Send(new GetEventIdsUserHasPermissions.Query(request.OrganizationId, request.UserId));
            List<Event> events = await eventRepository.GetEventsAsync([.. eventIds.Select(e => e.Key)]);

            foreach (var @event in events)
            {
                eventIds.TryGetValue(@event.Id, out EventPermissionLevel permissionLevel);

                @event.Series = [];
                @event.PermissionLevel = permissionLevel;
            }

            // Get the associated data
            List<AppUser> users = await mediator.Send(new GetUsersCurrentUserCanSee.Query(request.OrganizationId, TeamVisibility.TeamsUserOwns));
            List<Location> locations = await locationRepository.GetLocationsAsync(request.OrganizationId);
            List<Team> teams = await mediator.Send(new GetTeamsUserOwns.Query(request.OrganizationId));

            // Build the view
            EventsView view = new()
            {
                OrganizationId = request.OrganizationId,
                Events = events,
                AvailableUsersForPermissions = users,
                Locations = locations,
                Teams = teams
            };

            return Result.Success(view);
        }
    }
}