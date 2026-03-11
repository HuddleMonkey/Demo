using Demo.Application.Features.Events.Interfaces;
using Demo.Application.Features.Events.Models;
using Demo.Application.Features.Events.Queries;
using Demo.Application.Features.Media.Commands;
using Demo.Application.Features.Media.Models;
using Demo.Application.Features.Storage.Interfaces;

namespace Demo.Application.Features.Events.Commands;

/// <summary>
/// Updates an event logo, either by uploading an image or removing
/// </summary>
public class UpdateEventLogo
{
    /// <summary>
    /// Command
    /// </summary>
    /// <param name="eventId">Id of the event to update</param>
    /// <param name="imageBase64">If contains a value, the image to set as the logo. If null and event previously had a logo, the image will be removed.</param>
    public class Command(long eventId, string? imageBase64) : BaseRequest, IRequest<Result<string?>>
    {
        /// <summary>
        /// Id of the event to update
        /// </summary>
        public long EventId { get; init; } = eventId;

        /// <summary>
        /// If contains a value, the image to set as the logo. If null and event previously had a logo, the image will be removed.
        /// </summary>
        public string? ImageBase64 { get; init; } = imageBase64;
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.EventId).GreaterThan(0);
            RuleFor(x => x.CurrentUser).NotNull();
        }
    }

    public class Handler(IMediator mediator, IEventRepository eventRepository, IStorageService storageService, ILogger<Handler> logger) : IRequestHandler<Command, Result<string?>>
    {
        public async Task<Result<string?>> Handle(Command request, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Params: EventId={request.EventId}");
            string currentUserId = request.CurrentUserId;

            // Check that the user has access to edit this event
            bool hasAccess = await mediator.Send(new UserCanEditEvent.Query(request.EventId, currentUserId));
            if (!hasAccess) return Result.Failed<string?>("You do not have access to this event");

            // Get the event
            Event? @event = await eventRepository.GetEventAsync(request.EventId);
            if (@event is null) return Result.Failed<string?>("Event not found");

            // Remove existing logo from storage
            if (!string.IsNullOrEmpty(@event.LogoUrl))
            {
                Result<Empty> delete = await storageService.DeleteBlobInOrganizationFromUrlAsync(@event.OrganizationId, StorageDefaults.FolderEvents, @event.LogoUrl);
                if (delete.Failed) return Result.Failed<string?>(delete.Message);
            }

            // Add new image if provided
            if (!string.IsNullOrEmpty(request.ImageBase64))
            {
                Result<ImageProperties> logo = await mediator.Send(new ConvertBase64Image.Command(request.ImageBase64));
                if (logo.Failed || logo.Data is null) return Result.Failed<string?>(logo.Message);

                // Upload the graphic
                string logoName = $"logo-event-{@event.Id}.{logo.Data.FileExtension}";
                Result<string> resultLogo = await storageService.UploadBlobToOrganizationAsync(@event.OrganizationId, StorageDefaults.FolderEvents, logoName, logo.Data.ContentType, logo.Data.File!);
                if (resultLogo.Succeeded && !string.IsNullOrEmpty(resultLogo.Data))
                {
                    @event.LogoUrl = resultLogo.Data;
                }
                else
                {
                    return Result.Failed<string?>(resultLogo.Message);
                }
            }
            else
            {
                @event.LogoUrl = null;
            }

            @event = await eventRepository.SaveEntityAsync(@event, currentUserId);

            return Result.Success(@event.LogoUrl);
        }
    }
}
