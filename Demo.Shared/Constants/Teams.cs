namespace Demo.Shared.Constants;

/// <summary>
/// What teams the user can access
/// </summary>
public enum TeamVisibility
{
    TeamsUserOwns,
    TeamsInContainer
}

/// <summary>
/// What types of team data to include in the query
/// </summary>
[Flags]
public enum IncludeTeamProperties
{
    None = 0,
    Members = 1,
    Leaders = 2
}
