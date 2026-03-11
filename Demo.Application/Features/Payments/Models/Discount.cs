namespace Demo.Application.Features.Payments.Models;

/// <summary>
/// A discount represents the actual application of a coupon or promotion code. It contains information about when the 
/// discount began, when it will end, and what it is applied to.
/// </summary>
public class Discount
{
    /// <summary>
    /// The ID of the discount object
    /// </summary>
    public string Id { get; set; } = "";

    /// <summary>
    /// Coupon applied to create this discount
    /// </summary>
    public Coupon Coupon { get; set; } = new();

    /// <summary>
    /// Date that the coupon was applied.
    /// </summary>
    public DateTime Start { get; set; }

    /// <summary>
    /// If the coupon has a duration of repeating, the date that this discount will end. If the coupon 
    /// has a duration of once or forever, this attribute will be null.
    /// </summary>
    public DateTime? End { get; set; }
}
