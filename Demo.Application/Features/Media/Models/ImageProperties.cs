namespace Demo.Application.Features.Media.Models;

/// <summary>
/// Image Properties
/// </summary>
public class ImageProperties
{
    /// <summary>
    /// Stream of the file
    /// </summary>
    public MemoryStream? File { get; set; }

    /// <summary>
    /// File extension
    /// </summary>
    public string FileExtension { get; set; } = "";

    /// <summary>
    /// Content type of the file
    /// </summary>
    public string ContentType { get; set; } = "";

    /// <summary>
    /// Whether or not the file is valid
    /// </summary>
    public bool IsValid => File?.Length > 0;
}
