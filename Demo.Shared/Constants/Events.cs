namespace Demo.Shared.Constants;

/// <summary>
/// Event Permission Level
/// </summary>
public enum EventPermissionLevel
{
    None,
    Creator,
    AssignedPermission,
    TeamLeader
}

/// <summary>
/// Schedule status
/// </summary>
public enum ScheduleStatus
{
    NotSent,
    Invited,
    Accepted,
    Declined
}

/// <summary>
/// What types of event data to include in the query
/// </summary>
[Flags]
public enum IncludeEventProperties
{
    None = 0,
    Creator = 1,
    Positions = 2,
    PositionsDetails = 4,
    Series = 8,
    SeriesParts = 16,
    SeriesPartsWithSchedules = 32,
    SeriesPartsWithSchedulesAndUser = 64,
    SeriesAllDetails = 128,
    ScheduleTemplates = 256,
    ScheduleTemplatesWithPositions = 512,
    ScheduleTemplatesWithPositionsAndUsers = 1024
}

/// <summary>
/// What types of event position data to include in the query
/// </summary>
[Flags]
public enum IncludePositionProperties
{
    None = 0,
    Location = 1,
    Teams = 2,
    TeamsWithMembers = 4
}

/// <summary>
/// What types of event schedule data to include in the query
/// </summary>
[Flags]
public enum IncludeScheduleProperties
{
    None = 0,
    User = 1
}
