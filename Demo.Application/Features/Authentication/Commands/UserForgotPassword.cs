using Demo.Application.Features.Authentication.Models;
using Demo.Application.Features.Authentication.Notifications;
using Demo.Application.Features.Authentication.Queries;
using Demo.Application.Features.Users.Interfaces;
using Demo.Application.Features.Users.Models;
using Demo.Application.Features.Users.Queries;
using System.Net;

namespace Demo.Application.Features.Authentication.Commands;

/// <summary>
/// Sends an email to the user to reset their password.
/// </summary>
public class UserForgotPassword
{
    /// <summary>
    /// Command
    /// </summary>
    /// <param name="email">Email address of the user to send the reset password email to</param>
    public class Command(string email) : BaseRequest, IRequest<Result<Empty>>
    {
        /// <summary>
        /// Email address of the user to send the reset password email to
        /// </summary>
        public string Email { get; init; } = email;
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Email).NotEmpty().MaximumLength(256).EmailAddress();
        }
    }

    public class Handler(IMediator mediator, IUserRepository userRepository, ILogger<Handler> logger) : IRequestHandler<Command, Result<Empty>>
    {
        public async Task<Result<Empty>> Handle(Command request, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Params: Email={request.Email}");

            // Check to see if we have a valid user
            AppUser? user = await mediator.Send(new GetUserByEmail.Query(request.Email));
            if (user is null) return Result.Success<Empty>();

            // Verify that the user can login
            Result<CanUserLoginResponse> canUserLogin = await mediator.Send(new CanUserLogin.Query(user));
            if (canUserLogin.Failed) return Result.Success<Empty>();

            // Generate a code to use to verify the reset password request and build the callback Url
            string code = await userRepository.GeneratePasswordResetTokenAsync(user);
            string callbackUrl = $"{request.BaseWebUrl}/authentication/reset-password?code={WebUtility.UrlEncode(code)}";

            // Send notification to user with link
            await mediator.Publish(new SendUserResetPasswordCode(user, callbackUrl));

            return Result.Success<Empty>();
        }
    }
}
