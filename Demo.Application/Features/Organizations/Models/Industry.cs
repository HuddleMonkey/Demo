namespace Demo.Application.Features.Organizations.Models;

/// <summary>
/// Defines an industry
/// </summary>
public class Industry : BaseEntity
{
    /// <summary>
    /// The sort order of the industry
    /// </summary>
    public int Sequence { get; set; }

    /// <summary>
    /// Name of the industry
    /// </summary>
    public string Name { get; set; } = "";
}
