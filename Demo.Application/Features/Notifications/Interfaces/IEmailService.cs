using Demo.Application.Features.Users.Models;

namespace Demo.Application.Features.Notifications.Interfaces;

public interface IEmailService
{
    /// <summary>
    /// Sends an email to a user with a link to reset their password.
    /// </summary>
    /// <param name="user">User to send the message to</param>
    /// <param name="callbackUrl">Callback URL to reset their password</param>
    Task SendResetPasswordEmailAsync(AppUser user, string callbackUrl);
}
