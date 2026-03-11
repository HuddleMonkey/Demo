using Demo.Dto.Users;

namespace Demo.Dto.Teams;

public class TeamDto
{
    /// <summary>
    /// Team Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// The ID of the organization the team belongs to
    /// </summary>
    public long OrganizationId { get; set; }

    /// <summary>
    /// Name of the team
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Users that are members of the team
    /// </summary>
    public List<AppUserDto> Members { get; set; } = [];

    /// <summary>
    /// User Ids of any leaders for the team
    /// </summary>
    public List<string> LeaderUserIds { get; set; } = [];

    /// <summary>
    /// ID of the parent team (0 if no parent)
    /// </summary>
    public long? ParentId { get; set; }

    /// <summary>
    /// List of child teams (if any) of the team
    /// </summary>
    public List<TeamDto> Children { get; set; } = [];
}
