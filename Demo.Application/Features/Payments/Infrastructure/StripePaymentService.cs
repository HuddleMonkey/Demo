using Demo.Application.Domain.Settings;
using Demo.Application.Features.Payments.Interfaces;
using Demo.Application.Features.Storage.Interfaces;
using Stripe;
using HM = Demo.Application.Features.Payments.Models;

namespace Demo.Application.Features.Payments.Infrastructure;

public class StripePaymentService(ICacheService cache, IOptions<DemoSettings> settings, ILogger<StripePaymentService> logger) : IPaymentService
{
    private readonly DemoSettings _settings = settings.Value;

    /// <summary>
    /// Returns the Key to interface with the Payment Service
    /// </summary>
    /// <returns>Payment Service Key</returns>
    public string GetPaymentKey() => _settings.Payments.PublishableKey;

    /// <summary>
    /// Gets the number of days for a free trial.
    /// </summary>
    /// <returns>Days for free trial</returns>
    public int GetTrialPeriod() => 30;

    /// <summary>
    /// Gets a list of all active plans in Stripe
    /// </summary>
    /// <returns>List of Plans</returns>
    public async Task<List<HM.Plan>> GetPlansAsync()
    {
        logger.LogDebug("Params: none");

        try
        {
            List<HM.Plan>? dataPlans = cache.Get<List<HM.Plan>>("StripePlans");
            if (dataPlans is null)
            {
                dataPlans = [];
                PlanService service = new();
                PlanListOptions options = new()
                {
                    Active = true
                };

                StripeList<Plan> plans = await service.ListAsync(options);
                foreach (Plan plan in plans.Data)
                {
                    HM.Plan p = new()
                    {
                        Id = plan.Id,
                        Name = plan.Nickname,
                        Price = plan.Amount.HasValue ? (int)plan.Amount / 100 : 0,
                        MaxUsers = plan.Metadata.TryGetValue("MaxUsers", out string? maxUsers) ? Convert.ToInt32(maxUsers) : 1,
                        MaxStorage = plan.Metadata.TryGetValue("MaxStorage", out string? maxStorage) ? Convert.ToInt32(maxStorage) : 1
                    };
                    dataPlans.Add(p);
                }
                dataPlans = [.. dataPlans.OrderBy(p => p.Price)];
                dataPlans.RemoveAll(p => p.Name == "Enterprise");

                cache.Set("StripePlans", dataPlans);
            }

            return dataPlans;
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "An error occurred retrieving the plans");
            //SentrySdk.CaptureException(ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred retrieving the plans");
            //SentrySdk.CaptureException(ex);
        }

        return [];
    }

    /// <summary>
    /// Gets a subscription with the given ID
    /// </summary>
    /// <param name="id">ID of the subscription to get</param>
    /// <returns>Subscription</returns>
    public async Task<HM.Subscription?> GetSubscriptionAsync(string id)
    {
        logger.LogDebug($"Params: SubscriptionId={id}");
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));

        try
        {
            SubscriptionService service = new();
            SubscriptionGetOptions options = new() { Expand = ["discounts"] };
            Subscription stripeSubscription = await service.GetAsync(id, options) ?? new();
            SubscriptionItem item = stripeSubscription.Items.FirstOrDefault() ?? new();
            HM.Subscription subscription = new()
            {
                Id = stripeSubscription.Id,
                BillingCycleAnchor = stripeSubscription.BillingCycleAnchor,
                CurrentPeriodStart = item.CurrentPeriodStart,
                CurrentPeriodEnd = item.CurrentPeriodEnd,
                CancelAtPeriodEnd = stripeSubscription.CancelAtPeriodEnd,
                CanceledAt = stripeSubscription.CanceledAt,
                EndedAt = stripeSubscription.EndedAt,
                Start = stripeSubscription.StartDate,
                Status = stripeSubscription.Status,
                TrialStart = stripeSubscription.TrialStart,
                TrialEnd = stripeSubscription.TrialEnd,
                Discount = stripeSubscription.Discounts.Any() ? stripeSubscription.Discounts.First().Adapt<HM.Discount>() : null,
                Plan = new HM.Plan
                {
                    Id = item.Price.Id,
                    Name = item.Price.Nickname,
                    Price = (int)(item.Price.UnitAmount ?? 0) / 100,
                    MaxUsers = item.Price.Metadata.TryGetValue("MaxUsers", out string? maxUsers) ? Convert.ToInt32(maxUsers) : 1,
                    MaxStorage = item.Price.Metadata.TryGetValue("MaxStorage", out string? maxStorage) ? Convert.ToInt32(maxStorage) : 1
                }
            };

            return subscription;
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "An error occurred retrieving the subscription");
            //SentrySdk.CaptureException(ex);
        }
        catch (Exception ex)
        {
            logger.LogWarning($"Subscription {id} not found", ex);
            //SentrySdk.CaptureException(ex);
        }

        return null;
    }
}
