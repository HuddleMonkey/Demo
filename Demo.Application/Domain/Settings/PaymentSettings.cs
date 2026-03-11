namespace Demo.Application.Domain.Settings;

/// <summary>
/// Payment settings
/// </summary>
public class PaymentSettings
{
    /// <summary>
    /// ID of the default plan to select
    /// </summary>
    public string DefaultPlan { get; set; } = "";

    /// <summary>
    /// Key that is published to interact with the payment service
    /// </summary>
    public string PublishableKey { get; set; } = "";

    /// <summary>
    /// Secret key for accessing the payment service
    /// </summary>
    public string SecretKey { get; set; } = "";

    /// <summary>
    /// Key used to interact with the WebHooks
    /// </summary>
    public string WebHookKey { get; set; } = "";

    /// <summary>
    /// Flag to only process live web hooks
    /// </summary>
    public bool OnlyProcessLiveWebHooks { get; set; }
}
