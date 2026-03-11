using Demo.Application.Features.Users.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Application.Features.Teams.Models;

/// <summary>
/// Team
/// </summary>
public class Team : BaseEntity
{
    /// <summary>
    /// The ID of the organization the team belongs to
    /// </summary>
    public long OrganizationId { get; set; }

    /// <summary>
    /// Name of the team
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// ID of the parent team (0 if no parent)
    /// </summary>
    public long? ParentId { get; set; }

    /// <summary>
    /// If true, this group is at the top of the hierarchy and is considered a sub-container of groups
    /// </summary>
    public bool IsContainer { get; set; }

    /// <summary>
    /// Users that are members of the team
    /// </summary>
    public List<AppUser> Members { get; set; } = [];

    /// <summary>
    /// User Ids of any leaders for the team
    /// </summary>
    [NotMapped]
    public List<string> LeaderUserIds { get; set; } = [];

    /// <summary>
    /// Parent team (if any) of the team
    /// </summary>
    public Team? Parent { get; set; }

    /// <summary>
    /// List of child teams (if any) of the team
    /// </summary>
    public List<Team> Children { get; set; } = [];
}
