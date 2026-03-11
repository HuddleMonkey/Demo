using Demo.Application.Features.Events.Commands;
using Demo.Application.Features.Events.Models;
using Demo.Application.Features.Events.Queries;
using Demo.Dto.Events;
using Demo.Shared.Extensions;

namespace Demo.Api.Controllers.Events;

[Authorize]
public class EventsController(IMediator mediator) : ApiControllerBase
{
    [HttpGet("{organizationId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<EventsViewDto>))]
    public async Task<IActionResult> GetEvents(long organizationId)
    {
        var query = new GetEventsView.Query(organizationId, User.GetUserId());
        var result = await mediator.Send(query);

        return ObjectResult<EventsView, EventsViewDto>(result);
    }

    [HttpGet("view/{eventId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<EventViewDto>))]
    public async Task<IActionResult> GetEvent(long eventId)
    {
        var query = new GetEventView.Query(eventId);
        var result = await mediator.Send(query);

        return ObjectResult<EventView, EventViewDto>(result);
    }

    [Authorize(Policy = "ManageUsers")]
    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<EventDto>))]
    public async Task<IActionResult> CreateEvent(CreateEventRequest @event)
    {
        var command = new CreateEvent.Command(@event);
        var result = await mediator.Send(command);

        return ObjectResult<Event, EventDto>(result);
    }

    [HttpPost("update")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<EventDto>))]
    public async Task<IActionResult> UpdateEvent(UpdateEventRequest @event)
    {
        var command = new UpdateEvent.Command(@event);
        var result = await mediator.Send(command);

        return ObjectResult<Event, EventDto>(result);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Empty>))]
    public async Task<IActionResult> DeleteEvent(long id)
    {
        var command = new DeleteEvent.Command(id);
        var result = await mediator.Send(command);

        return ObjectResult(result);
    }
}
