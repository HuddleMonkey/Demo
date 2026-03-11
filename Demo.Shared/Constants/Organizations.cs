namespace Demo.Shared.Constants;

/// <summary>
/// Organization Status
/// </summary>
public enum OrganizationStatus
{
    Pending,
    Active,
    Suspended,
    Deleted
}

/// <summary>
/// What types of organization data to include in the query
/// </summary>
[Flags]
public enum IncludeOrganizationProperties
{
    None = 0,
    Industry = 1,
    Owner = 2,
    Users = 4,
    DeletedUsers = 8,
    Subscription = 16
}
