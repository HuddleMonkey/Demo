using Demo.Application.Features.Payments.Models;
using Demo.Application.Features.Users.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Application.Features.Organizations.Models;

/// <summary>
/// Organization
/// </summary>
public class Organization : BaseEntity
{
    /// <summary>
    /// Name of the organization
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Status of the organization
    /// </summary>
    public OrganizationStatus Status { get; set; } = OrganizationStatus.Pending;

    /// <summary>
    /// Message associated with the status the organization is in
    /// </summary>
    public string? StatusMessage { get; set; }

    /// <summary>
    /// Time zone the organization is operating in. If none is set, UTC is used.
    /// </summary>
    public string? TimeZone { get; set; }

    /// <summary>
    /// ID of the industry the organization is associated with
    /// </summary>
    public long IndustryId { get; set; } = 0;

    /// <summary>
    /// Industry model associated with the organization
    /// </summary>
    public virtual Industry? Industry { get; set; }

    /// <summary>
    /// Stripe Customer ID the organization is associated with
    /// </summary>
    public string? StripeCustomerId { get; set; }

    /// <summary>
    /// Stripe Subscription ID the organization is associated with
    /// </summary>
    public string? StripeSubscriptionId { get; set; }

    /// <summary>
    /// Stripe Plan ID the organization is signed up with
    /// </summary>
    public string? StripePlanId { get; set; }

    /// <summary>
    /// Optional promotion code.
    /// </summary>
    public string? PromotionCode { get; set; }

    /// <summary>
    /// Optional organization/person that referred this customer.
    /// </summary>
    public string? ReferredBy { get; set; }

    /// <summary>
    /// Gets the Subscription associated with the Organization
    /// </summary>
    [NotMapped]
    public Subscription? Subscription { get; set; }

    /// <summary>
    /// Text to display for how many users (not deleted) are registered out of the max allowed
    /// </summary>
    [NotMapped]
    public string UserLimitText
    {
        get
        {
            if (Subscription?.Plan is null) return "";
            if (Users is null) return "";

            int count = Users.Count(u => u.OrganizationUser?.Status != UserStatus.Deleted);

            if (Subscription.Plan.MaxUsers == 0) return $"{count} users registered";

            return $"{count} of {Subscription.Plan.MaxUsers} users registered";
        }
    }

    /// <summary>
    /// Checks to see if the max users (not deleted) registered has been reached based on the plan
    /// </summary>
    [NotMapped]
    public bool UserLimitReached
    {
        get
        {
            if (Subscription?.Plan is null) return true;
            if (Users is null) return true;
            if (Subscription.Plan.MaxUsers == 0) return false;
            int count = Users.Count(u => u.OrganizationUser?.Status != UserStatus.Deleted);

            return count >= Subscription.Plan.MaxUsers;
        }
    }

    /// <summary>
    /// Whether or not an organization has a plan that it can upgrade to
    /// </summary>
    [NotMapped]
    public bool CanUpgradePlan
    {
        get
        {
            if (Subscription?.Plan is null) return true;
            return Subscription.Plan.MaxUsers != 0;
        }
    }

    /// <summary>
    /// ID of the AppUser that owns the organization
    /// </summary>
    public string OwnerId { get; set; } = "";

    /// <summary>
    /// The user who is the owner of the organization account
    /// </summary>
    public virtual AppUser? Owner { get; set; }

    /// <summary>
    /// All users in the organization
    /// </summary>
    [NotMapped]
    public List<AppUser>? Users { get; set; }

    /// <summary>
    /// Date/Time the organization was created
    /// </summary>
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Last Activity Time
    /// </summary>
    [NotMapped]
    public DateTime? LastActivityTime { get; set; }

    /// <summary>
    /// Date/Time the organization account was deleted
    /// </summary>
    public DateTime? DateDeleted { get; set; }

    /// <summary>
    /// Reason the account was deleted
    /// </summary>
    public string? ReasonDeleted { get; set; }

    /// <summary>
    /// Comments provided when the account was deleted
    /// </summary>
    public string? Comments { get; set; }
}
