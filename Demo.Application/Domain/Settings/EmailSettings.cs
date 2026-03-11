namespace Demo.Application.Domain.Settings;

/// <summary>
/// Email settings
/// </summary>
public class EmailSettings
{
    /// <summary>
    /// Azure Communication Services Connection String
    /// </summary>
    public string AzureCommunicationServicesConnectionString { get; set; } = "";

    /// <summary>
    /// Base URL to use for callback URLs in email. The URL does not contain a trailing "/"
    /// </summary>
    public string BaseUrl { get; set; } = "";

    /// <summary>
    /// Email addresss the email is sent from
    /// </summary>
    public string FromEmail { get; set; } = "";

    /// <summary>
    /// Display name the email is sent from
    /// </summary>
    public string FromName { get; set; } = "";

    /// <summary>
    /// Email address to send all admin email to
    /// </summary>
    public string AdminEmail { get; set; } = "";

    /// <summary>
    /// Name of the admin
    /// </summary>
    public string AdminName { get; set; } = "";

    /// <summary>
    /// Whether the system is running in demo mode - if true, all emails are sent to the DemoSendToEmail
    /// </summary>
    public bool DemoMode { get; set; }

    /// <summary>
    /// Email address to send all emails to if in demo mode
    /// </summary>
    public string DemoSendToEmail { get; set; } = "";

    /// <summary>
    /// Whether or not email is enabled
    /// </summary>
    public bool Enabled { get; set; }
}
