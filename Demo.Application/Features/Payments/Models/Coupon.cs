namespace Demo.Application.Features.Payments.Models;

/// <summary>
/// A coupon contains information about a percent-off or amount-off discount you might want to apply to a customer. 
/// </summary>
public class Coupon
{
    /// <summary>
    /// Unique identifier for the object.
    /// </summary>
    public string Id { get; set; } = "";

    /// <summary>
    /// Name of the coupon displayed to customers on for instance invoices or receipts.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Amount (in the <c>currency</c> specified) that will be taken off the subtotal of any
    /// invoices for this customer.
    /// </summary>
    public long? AmountOff { get; set; }

    /// <summary>
    /// Percent that will be taken off the subtotal of any invoices for this customer for the
    /// duration of the coupon. For example, a coupon with percent_off of 50 will make a %s100
    /// invoice %s50 instead.
    /// </summary>
    public decimal? PercentOff { get; set; }

    /// <summary>
    /// One of <c>forever</c>, <c>once</c>, and <c>repeating</c>. Describes how long a customer
    /// who applies this coupon will get the discount.
    /// One of: <c>forever</c>, <c>once</c>, or <c>repeating</c>.
    /// </summary>
    public string Duration { get; set; } = "";

    /// <summary>
    /// If <c>duration</c> is <c>repeating</c>, the number of months the coupon applies. Null if
    /// coupon <c>duration</c> is <c>forever</c> or <c>once</c>.
    /// </summary>
    public long? DurationInMonths { get; set; }

    /// <summary>
    /// Maximum number of times this coupon can be redeemed, in total, across all customers,
    /// before it is no longer valid.
    /// </summary>
    public long? MaxRedemptions { get; set; }

    /// <summary>
    /// Number of times this coupon has been applied to a customer.
    /// </summary>
    public long TimesRedeemed { get; set; }

    /// <summary>
    /// Date after which the coupon can no longer be redeemed.
    /// </summary>
    public DateTime? RedeemBy { get; set; }

    /// <summary>
    /// Taking account of the above properties, whether this coupon can still be applied to a customer.
    /// </summary>
    public bool Valid { get; set; }

    /// <summary>
    /// Terms of the coupon
    /// </summary>
    public string Terms
    {
        get
        {
            string terms = "";

            if (AmountOff.HasValue)
            {
                terms = string.Format("{0:C}", AmountOff / 100);
            }
            else if (PercentOff.HasValue)
            {
                terms = $"{PercentOff}%";
            }

            terms += " off ";

            if (Duration == "once" || Duration == "forever")
            {
                terms += Duration;
            }
            else if (DurationInMonths.HasValue)
            {
                terms += $"for {DurationInMonths} month";
                if (DurationInMonths > 1)
                {
                    terms += "s";
                }
            }

            return terms.Trim();
        }
    }
}
