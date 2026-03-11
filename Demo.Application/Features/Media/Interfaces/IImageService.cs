using Demo.Application.Features.Media.Models;

namespace Demo.Application.Features.Media.Interfaces;

public interface IImageService
{
    /// <summary>
    /// Converts a byte array image to a stream.
    /// </summary>
    /// <param name="imageArray">Byte array of the image</param>
    /// <param name="maxWidth">Max width of the image to generate. Default is 1024.</param>
    /// <returns>ImageProperties</returns>
    ImageProperties ConvertImage(byte[] imageArray, int maxWidth = 1024);
}
