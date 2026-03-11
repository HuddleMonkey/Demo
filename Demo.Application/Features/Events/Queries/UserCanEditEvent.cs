using Demo.Application.Features.Events.Interfaces;
using Demo.Application.Features.Events.Models;

namespace Demo.Application.Features.Events.Queries;

/// <summary>
/// Checks if the user can edit the event - either creator or has permission to access
/// </summary>
public class UserCanEditEvent
{
    /// <summary>
    /// Query
    /// </summary>
    /// <param name="eventId">Id of the event to check</param>
    /// <param name="userId">Id of the user to check</param>
    public class Query(long eventId, string userId) : BaseRequest, IRequest<bool>
    {
        /// <summary>
        /// Id of the event to check
        /// </summary>
        public long EventId { get; init; } = eventId;

        /// <summary>
        /// Id of the user to check
        /// </summary>
        public string UserId { get; init; } = userId;
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.EventId).GreaterThan(0);
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.CurrentUser).NotNull();
        }
    }

    public class Handler(IEventRepository eventRepository, /*IEventPermissionRepository eventPermissionRepository,*/ ILogger<Handler> logger) : IRequestHandler<Query, bool>
    {
        public async Task<bool> Handle(Query request, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Params: EventId={request.EventId}, userId={request.UserId}");

            // Get events user created
            List<Event> created = await eventRepository.GetEventsUserCreatedAsync(request.UserId);
            if (created.Any(e => e.Id == request.EventId))
            {
                return true;
            }

            // Get event user has permission to see
            //EventPermission? permission = await eventPermissionRepository.GetEventUserHasAccessToAsync(request.EventId, request.UserId);
            //if (permission is not null)
            //{
            //    return true;
            //}

            return false;
        }
    }
}
