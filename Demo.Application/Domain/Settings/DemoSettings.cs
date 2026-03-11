namespace Demo.Application.Domain.Settings;

/// <summary>
/// Global Settings
/// </summary>
public class DemoSettings
{
    /// <summary>
    /// Keys
    /// </summary>
    public KeySettings Keys { get; set; } = new();

    /// <summary>
    /// Email settings
    /// </summary>
    public EmailSettings Email { get; set; } = new();

    /// <summary>
    /// Token settings
    /// </summary>
    public TokenSettings Token { get; set; } = new();

    /// <summary>
    /// Payment settings
    /// </summary>
    public PaymentSettings Payments { get; set; } = new();

    /// <summary>
    /// Storage settings
    /// </summary>
    public StorageSettings Storage { get; set; } = new();
}
