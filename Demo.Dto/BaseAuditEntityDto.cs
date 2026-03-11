using Demo.Dto.Users;

namespace Demo.Dto;

/// <summary>
/// Base entity for objects that track audit (created by, modified by, etc)
/// </summary>
public abstract class BaseAuditEntityDto : BaseEntityDto
{
    /// <summary>
    /// Date/time the item was created
    /// </summary>
    public DateTime CreatedOn { get; set; } = DateTime.MinValue;

    /// <summary>
    /// User who created the item
    /// </summary>
    public AppUserDto? CreatedBy { get; set; }

    /// <summary>
    /// Date/time the item was modified
    /// </summary>
    public DateTime? ModifiedOn { get; set; }

    /// <summary>
    /// User who modified the item
    /// </summary>
    public AppUserDto? ModifiedBy { get; set; }
}
