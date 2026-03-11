namespace Demo.Application.Features.Payments.Models;

/// <summary>
/// Defines the subscription for the organization
/// </summary>
public class Subscription
{
    /// <summary>
    /// ID of the subscription
    /// </summary>
    public string Id { get; set; } = "";

    /// <summary>
    /// Determines the date of the first full invoice, and, for plans with month or year intervals, the day of the month for subsequent invoices.
    /// </summary>
    public DateTime? BillingCycleAnchor { get; set; }

    /// <summary>
    /// If the subscription has been canceled with the at_period_end flag set to true, cancel_at_period_end on the subscription will be true. You can use 
    /// this attribute to determine whether a subscription that has a status of active is scheduled to be canceled at the end of the current period.
    /// </summary>
    public bool CancelAtPeriodEnd { get; set; }

    /// <summary>
    /// If the subscription has been canceled, the date of that cancellation. If the subscription was canceled with cancel_at_period_end, canceled_at will still 
    /// reflect the date of the initial cancellation request, not the end of the subscription period when the subscription is automatically moved to a canceled state.
    /// </summary>
    public DateTime? CanceledAt { get; set; }

    /// <summary>
    /// Start of the current period that the subscription has been invoiced for.
    /// </summary>
    public DateTime? CurrentPeriodStart { get; set; }

    /// <summary>
    /// End of the current period that the subscription has been invoiced for. At the end of this period, a new invoice will be created.
    /// </summary>
    public DateTime? CurrentPeriodEnd { get; set; }

    /// <summary>
    /// If the subscription has ended, the date the subscription ended.
    /// </summary>
    public DateTime? EndedAt { get; set; }

    /// <summary>
    /// Date of the last substantial change to this subscription. For example, a change to the items array, or a change of status, will reset this timestamp.
    /// </summary>
    public DateTime? Start { get; set; }

    /// <summary>
    /// Possible values are trialing, active, past_due, canceled, or unpaid.
    /// </summary>
    public string Status { get; set; } = "";

    /// <summary>
    /// If the subscription has a trial, the beginning of that trial.
    /// </summary>
    public DateTime? TrialStart { get; set; }

    /// <summary>
    /// If the subscription has a trial, the end of that trial.
    /// </summary>
    public DateTime? TrialEnd { get; set; }

    /// <summary>
    /// If in a trial, gets the number of days until the trial ends. (-1 if not in trial)
    /// </summary>
    public int DaysUntilTrialEnds
    {
        get
        {
            if (Status == "trialing" && TrialEnd.HasValue)
            {
                double days = (TrialEnd.Value - DateTime.Now).TotalDays;
                return (int)days;
            }

            return -1;
        }
    }

    /// <summary>
    /// Plan associated with the subscription
    /// </summary>
    public Plan Plan { get; set; } = new();

    /// <summary>
    /// Optional discount that is applied
    /// </summary>
    public Discount? Discount { get; set; }

    /// <summary>
    /// Monthly price paid by organization, to account for plan and any discounts applied
    /// </summary>
    public long MonthlyPrice
    {
        get
        {
            long price = Plan.Price;

            if (Discount?.Coupon is not null && Discount.Coupon.Valid)
            {
                if (Discount.Coupon.AmountOff.HasValue)
                {
                    price -= Discount.Coupon.AmountOff.Value / 100;
                }
                else if (Discount.Coupon.PercentOff.HasValue)
                {
                    var percentage = Discount.Coupon.PercentOff.Value / 100;
                    var discount = price * percentage;

                    price -= Convert.ToInt64(discount);
                }
            }

            return price;
        }
    }
}
