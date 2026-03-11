using Demo.Application.Features.Events.Interfaces;
using Demo.Application.Features.Events.Models;
using Demo.Application.Features.Events.Queries;
using Demo.Dto.Events;

namespace Demo.Application.Features.Events.Commands;

/// <summary>
/// Updates an event
/// </summary>
public class UpdateEvent
{
    /// <summary>
    /// Command
    /// </summary>
    /// <param name="event">Details of the event to update</param>
    public class Command(UpdateEventRequest @event) : BaseRequest, IRequest<Result<Event>>
    {
        /// <summary>
        /// Details of the event to update
        /// </summary>
        public UpdateEventRequest Event { get; init; } = @event;
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Event).NotNull();
            RuleFor(x => x.Event.EventId).GreaterThan(0);
            RuleFor(x => x.Event.Name).NotEmpty().MaximumLength(300);
            RuleFor(x => x.Event.Description).MaximumLength(5000);
            RuleFor(x => x.Event.Recurring).IsInEnum();
            RuleFor(x => x.Event.CustomFrequency).NotNull().When(x => x.Event.Recurring == Recurring.Custom);
            RuleFor(x => x.Event.CustomTimeUnit).NotNull().When(x => x.Event.Recurring == Recurring.Custom);
            RuleFor(x => x.Event.CustomDayOfWeek).NotNull().When(x => x.Event.Recurring == Recurring.Custom && x.Event.CustomTimeUnit == RecurringTimeUnit.Weeks);
            RuleFor(x => x.CurrentUser).NotNull();
        }
    }

    public class Handler(IMediator mediator, IEventRepository eventRepository, ILogger<Handler> logger) : IRequestHandler<Command, Result<Event>>
    {
        public async Task<Result<Event>> Handle(Command request, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Params: Name={request.Event.Name}");
            string currentUserId = request.CurrentUser!.GetUserId();

            // Check that the user has access to edit this event
            bool hasAccess = await mediator.Send(new UserCanEditEvent.Query(request.Event.EventId, currentUserId));
            if (!hasAccess) return Result.Failed<Event>("You do not have access to this event");

            // Get the event
            Event? @event = await eventRepository.GetEventAsync(request.Event.EventId);
            if (@event is null) return Result.Failed<Event>("Event not found");

            bool nameChanged = @event.Name != request.Event.Name;

            // Update the event
            @event.Name = request.Event.Name;
            @event.Description = request.Event.Description;
            @event.Recurring = request.Event.Recurring;
            @event.CustomFrequency = request.Event.Recurring == Recurring.Custom ? request.Event.CustomFrequency : null;
            @event.CustomTimeUnit = request.Event.Recurring == Recurring.Custom ? request.Event.CustomTimeUnit : null;
            @event.CustomDayOfWeek = request.Event.Recurring == Recurring.Custom && request.Event.CustomTimeUnit == RecurringTimeUnit.Weeks ? request.Event.CustomDayOfWeek : null;

            @event = await eventRepository.SaveEntityAsync(@event, currentUserId);

            // If the name changed, update any bookmarks
            //if (nameChanged)
            //{
            //    await mediator.Send(new UpdateBookmarkName.Command(BookmarkType.Event, @event.Id, alternateTypeId: null, @event.Name));
            //}

            return Result.Success(@event);
        }
    }
}
