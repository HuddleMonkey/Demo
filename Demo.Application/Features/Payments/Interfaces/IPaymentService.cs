using Demo.Application.Features.Payments.Models;

namespace Demo.Application.Features.Payments.Interfaces;

public interface IPaymentService
{
    /// <summary>
    /// Returns the Key to interface with the Payment Service
    /// </summary>
    /// <returns>Payment Service Key</returns>
    string GetPaymentKey();

    /// <summary>
    /// Gets the number of days for a free trial.
    /// </summary>
    /// <returns>Days for free trial</returns>
    int GetTrialPeriod();

    /// <summary>
    /// Gets a list of all active plans in Stripe
    /// </summary>
    /// <returns>List of Plans</returns>
    Task<List<Plan>> GetPlansAsync();

    /// <summary>
    /// Gets a subscription with the given ID
    /// </summary>
    /// <param name="id">ID of the subscription to get</param>
    /// <returns>Subscription</returns>
    Task<Subscription?> GetSubscriptionAsync(string id);
}
