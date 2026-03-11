using Demo.Application.Features.Media.Interfaces;
using Demo.Application.Features.Media.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Demo.Application.Features.Media.Infrastructure;

public class ImageSharpImageService(ILogger<ImageSharpImageService> logger) : IImageService
{
    /// <summary>
    /// Converts a byte array image to a stream.
    /// </summary>
    /// <param name="imageArray">Byte array of the image</param>
    /// <param name="maxWidth">Max width of the image to generate. Default is 1024.</param>
    /// <returns>ImageProperties</returns>
    public ImageProperties ConvertImage(byte[] imageArray, int maxWidth = 1024)
    {
        logger.LogDebug($"Params: imageArray length={imageArray?.Length}, maxWidth={maxWidth}");
        ArgumentNullException.ThrowIfNull(imageArray);

        try
        {
            var outputStream = new MemoryStream();
            using var image = Image.Load(imageArray);
            string fileExtension = "";
            string contentType = "";

            if (image.Width > maxWidth)
            {
                image.Mutate(i => i.Resize(maxWidth, ((maxWidth * image.Height) / image.Width)));
            }

            if (image.Metadata.DecodedImageFormat is not null)
            {
                fileExtension = image.Metadata.DecodedImageFormat.FileExtensions.FirstOrDefault() ?? "";
                contentType = image.Metadata.DecodedImageFormat.DefaultMimeType;
                image.Save(outputStream, image.Metadata.DecodedImageFormat);
            }
            else
            {
                fileExtension = "png";
                contentType = "image/png";
                image.SaveAsPng(outputStream);
            }

            outputStream.Seek(0, SeekOrigin.Begin);

            ImageProperties imageProperties = new()
            {
                File = outputStream,
                FileExtension = fileExtension,
                ContentType = contentType
            };

            return imageProperties;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred converting the image");
            return new();
        }
    }
}