using Demo.Application.Features.Users.Models;

namespace Demo.Application.Domain;

/// <summary>
/// Base entity for objects that track audit (created by, modified by, etc)
/// </summary>
public abstract class BaseAuditEntity : BaseEntity
{
    /// <summary>
    /// Date/time the item was created
    /// </summary>
    public DateTime CreatedOn { get; set; } = DateTime.MinValue;

    /// <summary>
    /// User ID that created the item
    /// </summary>
    public string CreatedByUserId { get; set; } = "";

    /// <summary>
    /// User who created the item
    /// </summary>
    public virtual AppUser? CreatedBy { get; set; }

    /// <summary>
    /// Date/time the item was modified
    /// </summary>
    public DateTime? ModifiedOn { get; set; }

    /// <summary>
    /// User ID that modified the item
    /// </summary>
    public string? ModifiedByUserId { get; set; }

    /// <summary>
    /// User who modified the item
    /// </summary>
    public virtual AppUser? ModifiedBy { get; set; }
}
