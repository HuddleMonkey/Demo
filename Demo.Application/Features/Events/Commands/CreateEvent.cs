using Demo.Application.Features.Events.Interfaces;
using Demo.Application.Features.Events.Models;
using Demo.Application.Features.Events.Queries;
using Demo.Dto.Events;

namespace Demo.Application.Features.Events.Commands;

/// <summary>
/// Creates a new event
/// </summary>
public class CreateEvent
{
    /// <summary>
    /// Command
    /// </summary>
    /// <param name="event">Details of the event to create</param>
    public class Command(CreateEventRequest @event) : BaseRequest, IRequest<Result<Event>>
    {
        /// <summary>
        /// Details of the event to create
        /// </summary>
        public CreateEventRequest Event { get; init; } = @event;
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Event).NotNull();
            RuleFor(x => x.Event.OrganizationId).GreaterThan(0);
            RuleFor(x => x.Event.Name).NotEmpty().MaximumLength(300);
            RuleFor(x => x.Event.Description).MaximumLength(5000);
            RuleFor(x => x.Event.Recurring).IsInEnum();
            RuleFor(x => x.Event.CustomFrequency).NotNull().When(x => x.Event.Recurring == Recurring.Custom);
            RuleFor(x => x.Event.CustomTimeUnit).NotNull().When(x => x.Event.Recurring == Recurring.Custom);
            RuleFor(x => x.Event.CustomDayOfWeek).NotNull().When(x => x.Event.Recurring == Recurring.Custom && x.Event.CustomTimeUnit == RecurringTimeUnit.Weeks);
            RuleFor(x => x.Event.PermissionUserIds).NotNull();
            RuleFor(x => x.Event.Positions).NotNull();
            RuleFor(x => x.CurrentUser).NotNull();
        }
    }

    public class Handler(IMediator mediator, IEventRepository eventRepository, IPositionRepository positionRepository, ILogger<Handler> logger) : IRequestHandler<Command, Result<Event>>
    {
        public async Task<Result<Event>> Handle(Command request, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Params: Name={request.Event.Name}");

            // Check if they are a member of the organization
            if (!request.CurrentUser!.IsInOrganization(request.Event.OrganizationId))
            {
                return Result.Failed<Event>("You are not a member of the organization");
            }

            // Create the event
            Event @event = new()
            {
                OrganizationId = request.Event.OrganizationId,
                Name = request.Event.Name,
                Description = request.Event.Description,
                Recurring = request.Event.Recurring,
                CustomFrequency = request.Event.Recurring == Recurring.Custom ? request.Event.CustomFrequency : null,
                CustomTimeUnit = request.Event.Recurring == Recurring.Custom ? request.Event.CustomTimeUnit : null,
                CustomDayOfWeek = request.Event.Recurring == Recurring.Custom && request.Event.CustomTimeUnit == RecurringTimeUnit.Weeks ? request.Event.CustomDayOfWeek : null,
                RemindNotRespondedInterval = request.Event.RemindNotRespondedInterval,
                RemindNotRespondedEventThreshold = request.Event.RemindNotRespondedEventThreshold,
                RemindUpcomingSchedule = string.Join('|', request.Event.RemindUpcomingScheduleDays.Distinct().Order())
            };
            @event = await eventRepository.SaveEntityAsync(@event, request.CurrentUserId);
            @event.PermissionLevel = EventPermissionLevel.Creator;

            if (@event.Id > 0)
            {
                // Set the event logo if one was uploaded
                if (!string.IsNullOrEmpty(request.Event.ImageBase64))
                {
                    Result<string?> resultLogo = await mediator.Send(new UpdateEventLogo.Command(@event.Id, request.Event.ImageBase64));
                    @event.LogoUrl = resultLogo.Data;
                }

                // Add any positions
                if (request.Event.Positions.Any())
                {
                    await AddPositionsAsync(request.Event.OrganizationId, @event.Id, request.Event.Positions);
                }

                // Add any optional permissions
                //if (request.Event.PermissionUserIds.Any())
                //{
                //    await mediator.Send(new AddEventPermissions.Command(@event.Id, request.Event.PermissionUserIds));
                //}
            }

            return Result.Success(@event);
        }

        /// <summary>
        /// Add the positions to the event
        /// </summary>
        /// <param name="organizationId">Associated organization id</param>
        /// <param name="eventId">Id of the associated event</param>
        /// <param name="positions">Positions to add</param>
        private async Task AddPositionsAsync(long organizationId, long eventId, List<PositionDto> positions)
        {
            // Get the locations
            List<Location> locations = await mediator.Send(new GetLocationsFromPositions.Query(organizationId, positions));

            // Create the positions
            List<Position> positionsToAdd = [];
            foreach (var position in positions)
            {
                // Create the position
                Position newPosition = new()
                {
                    EventId = eventId,
                    LocationId = locations.FirstOrDefault(l => l.Name == position.Location?.Name)?.Id,
                    StartTime = position.StartTime,
                    EndTime = position.EndTime,
                    Category = position.Category,
                    Name = position.Name,
                    Description = position.Description,
                    NumberRequired = position.NumberRequired
                };

                // Add any teams associated with the position
                //if (position.SchedulingRestriction == SchedulingRestriction.Teams && position.Teams.Any())
                //{
                //    foreach (var allowedTeam in position.Teams)
                //    {
                //        PositionTeam team = new()
                //        {
                //            TeamId = allowedTeam.Id
                //        };
                //        newPosition.PositionTeams.Add(team);
                //    }
                //}

                positionsToAdd.Add(newPosition);
            }

            await positionRepository.SaveEntitiesAsync(positionsToAdd);
        }
    }
}
