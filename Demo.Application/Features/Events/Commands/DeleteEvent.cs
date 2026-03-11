using Demo.Application.Features.Events.Interfaces;
using Demo.Application.Features.Events.Models;
using Demo.Application.Features.Events.Queries;
using Demo.Application.Features.Storage.Interfaces;

namespace Demo.Application.Features.Events.Commands;

/// <summary>
/// Deletes an event
/// </summary>
public class DeleteEvent
{
    /// <summary>
    /// Command
    /// </summary>
    /// <param name="eventId">Id of the event to delete</param>
    public class Command(long eventId) : BaseRequest, IRequest<Result<Empty>>
    {
        /// <summary>
        /// Id of the series to remove the conversation from
        /// </summary>
        public long EventId { get; init; } = eventId;
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.EventId).GreaterThan(0);
            RuleFor(x => x.CurrentUser).NotNull();
        }
    }

    public class Handler(IMediator mediator, IEventRepository eventRepository, /*IEventPermissionRepository eventPermissionRepository,*/ IPositionRepository positionRepository, IScheduleRepository scheduleRepository, /*ISeriesContentRepository seriesContentRepository,*/ IStorageService storageService, ILogger<Handler> logger) : IRequestHandler<Command, Result<Empty>>
    {
        public async Task<Result<Empty>> Handle(Command request, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Params: EventId={request.EventId}");

            // Check that the user has access to edit this event
            bool hasAccess = await mediator.Send(new UserCanEditEvent.Query(request.EventId, request.CurrentUser!.GetUserId()));
            if (!hasAccess) return Result.Failed<Empty>("You do not have access to this event");

            // Clear out the positions in content and schedules
            List<Position> positions = await positionRepository.GetPositionsAsync(request.EventId);
            List<long> positionIds = [.. positions.Select(p => p.Id)];

            List<Schedule> schedules = await scheduleRepository.GetSchedulesWithPositionsAsync(positionIds);
            schedules.ForEach(s => s.PositionId = null);
            await scheduleRepository.SaveEntitiesAsync(schedules);

            //List<SeriesContent> content = await seriesContentRepository.GetSeriesContentByPositionIdsAsync(positionIds);
            //content.ForEach(c => c.PositionId = 0);
            //await seriesContentRepository.SaveEntitiesAsync(content);

            // Get the event
            Event? @event = await eventRepository.GetEventAsync(request.EventId, IncludeEventProperties.PositionsDetails | IncludeEventProperties.SeriesAllDetails | IncludeEventProperties.ScheduleTemplatesWithPositions);
            if (@event is null) return Result.Failed<Empty>("Event not found");

            // Delete Permissions
            //List<EventPermission> permissions = await eventPermissionRepository.GetEventPermissionsAsync(@event.Id);
            //if (permissions.Any())
            //{
            //    await eventPermissionRepository.DeleteEntitiesAsync(permissions);
            //}

            // Delete the logos
            List<string> logos = [];
            if (!string.IsNullOrEmpty(@event.LogoUrl)) logos.Add(@event.LogoUrl);
            logos.AddRange(@event.Series.Where(s => !string.IsNullOrEmpty(s.LogoUrl)).Select(s => s.LogoUrl!));
            logos = [.. logos.Distinct()];
            foreach (var logo in logos)
            {
                await storageService.DeleteBlobInOrganizationFromUrlAsync(@event.OrganizationId, StorageDefaults.FolderEvents, logo);
            }

            // Delete the event
            await eventRepository.DeleteEntityAsync(@event);

            // Delete any bookmarks
            //await mediator.Send(new DeleteBookmarks.Command(BookmarkType.Event, @event.Id, alternateTypeId: null, organizationId: @event.OrganizationId));

            return Result.Success<Empty>();
        }
    }
}
