namespace Demo.Application.Features.Payments.Models;

/// <summary>
/// Defines a plan an organization can subscribed to
/// </summary>
public class Plan
{
    /// <summary>
    /// ID of the plan
    /// </summary>
    public string Id { get; set; } = "";

    /// <summary>
    /// Display name of the plan
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Price of plan in dollars (monthly)
    /// </summary>
    public int Price { get; set; }

    /// <summary>
    /// Max number of users allowed under the plan. Set to 0 if unlimited.
    /// </summary>
    public int MaxUsers { get; set; }

    /// <summary>
    /// Max amount of storage, in GB, allowed under the plan.
    /// </summary>
    public int MaxStorage { get; set; }
}
