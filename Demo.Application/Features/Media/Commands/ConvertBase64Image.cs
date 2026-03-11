using Demo.Application.Features.Media.Interfaces;
using Demo.Application.Features.Media.Models;
using Humanizer;

namespace Demo.Application.Features.Media.Commands;

/// <summary>
/// Converts a Base64 image to ImageProperties
/// </summary>
public class ConvertBase64Image
{
    /// <summary>
    /// Command
    /// </summary>
    /// <param name="base64">Image in Base64</param>
    /// <param name="maxFileSize">Max file size, in bytes (default is 10MB)</param>
    public class Command(string base64, long maxFileSize = UploadSettings.MaxAttachmentFileSize) : BaseRequest, IRequest<Result<ImageProperties>>
    {
        /// <summary>
        /// Image in Base64
        /// </summary>
        public string Base64 { get; init; } = base64;

        /// <summary>
        /// Max file size, in bytes (default is 10MB)
        /// </summary>
        public long MaxFileSize { get; init; } = maxFileSize;
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Base64).NotEmpty();
            RuleFor(x => x.MaxFileSize).GreaterThan(0);
        }
    }

    public class Handler(IImageService imageService, ILogger<Handler> logger) : IRequestHandler<Command, Result<ImageProperties>>
    {
        public async Task<Result<ImageProperties>> Handle(Command request, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Params: Base64={request.Base64}");
            await Task.Delay(0);

            // Convert base64 to bytes
            byte[] bytes = Convert.FromBase64String(request.Base64);

            // Convert to a stream and get the image properties
            ImageProperties image = imageService.ConvertImage(bytes);
            if (image.File is null) return Result.Failed<ImageProperties>("Error occurred uploading the graphic.");
            if (image.ContentType != @"image/jpeg" && image.ContentType != @"image/gif" && image.ContentType != @"image/png") return Result.Failed<ImageProperties>($"File format {image.ContentType} is not supported. Please upload a gif, jpeg, or png.");
            if (image.File.Length > request.MaxFileSize) return Result.Failed<ImageProperties>($"Images cannot exceed {request.MaxFileSize.Bytes()}");

            return Result.Success(image);
        }
    }
}
