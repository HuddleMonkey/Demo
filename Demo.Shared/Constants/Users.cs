namespace Demo.Shared.Constants;

/// <summary>
/// What types of user data to include in the query
/// </summary>
[Flags]
public enum IncludeUserProperties
{
    None = 0,
    Teams = 1,
    OrganizationUser = 2,
    ProviderUser = 4
}

/// <summary>
/// What types of organization user data to include in the query
/// </summary>
[Flags]
public enum IncludeOrganizationUserProperties
{
    None = 0,
    DeletedUsers = 1
}

/// <summary>
/// User Status
/// </summary>
public enum UserStatus
{
    Invited,
    Active,
    Deleted
}

/// <summary>
/// Defines the different roles available
/// </summary>
public static class DemoRoles
{
    public const string Admin = "Admin";
}

/// <summary>
/// Defines the different claims available
/// </summary>
public static class DemoClaimTypes
{
    public const string Organization = "Organization";
    public const string OrganizationOwner = "OrganizationOwner";
    public const string OrganizationAdmin = "OrganizationAdmin";
    public const string TeamLeader = "TeamLeader";
    public const string Provider = "Provider";
}
