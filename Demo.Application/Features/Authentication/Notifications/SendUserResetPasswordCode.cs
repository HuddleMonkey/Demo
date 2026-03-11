using Demo.Application.Features.Notifications.Interfaces;
using Demo.Application.Features.Users.Models;

namespace Demo.Application.Features.Authentication.Notifications;

/// <summary>
/// Notification that sends the callback code to reset their password
/// </summary>
/// <param name="user">User who is requesting the password reset</param>
/// <param name="callbackUrl">Callback url with the link to reset their password</param>
public class SendUserResetPasswordCode(AppUser user, string callbackUrl) : INotification
{
    /// <summary>
    /// User who is requesting the password reset
    /// </summary>
    public AppUser User { get; init; } = user;

    /// <summary>
    /// Callback url with the link to reset their password
    /// </summary>
    public string CallbackUrl { get; init; } = callbackUrl;
}

/// <summary>
/// Sends the email to the user with the link to reset their password
/// </summary>
public class SendUserResetPasswordCodeEmail(IEmailService emailService, ILogger<SendUserResetPasswordCodeEmail> logger) : INotificationHandler<SendUserResetPasswordCode>
{
    public async Task Handle(SendUserResetPasswordCode notification, CancellationToken cancellationToken)
    {
        logger.LogDebug($"Params: UserId={notification.User.Id}");

        await emailService.SendResetPasswordEmailAsync(notification.User, notification.CallbackUrl);
    }
}
