using Demo.Application.Features.Users.Interfaces;
using Demo.Application.Features.Users.Models;
using Demo.Application.Features.Users.Queries;
using System.Net;

namespace Demo.Application.Features.Authentication.Commands;

/// <summary>
/// Resets the user's password.
/// </summary>
public class UserResetPassword
{
    /// <summary>
    /// Command
    /// </summary>
    /// <param name="email">Email of the user resetting their password</param>
    /// <param name="password">New password request</param>
    /// <param name="confirmPassword">Confirming the new password</param>
    /// <param name="code">Token/Code used to validate the password reset</param>
    public class Command(string email, string password, string confirmPassword, string code) : BaseRequest, IRequest<Result<Empty>>
    {
        /// <summary>
        /// Email of the user resetting their password
        /// </summary>
        public string Email { get; init; } = email;

        /// <summary>
        /// New password request
        /// </summary>
        public string Password { get; init; } = password;

        /// <summary>
        /// Confirming the new password
        /// </summary>
        public string ConfirmPassword { get; init; } = confirmPassword;

        /// <summary>
        /// Token/Code used to validate the password reset
        /// </summary>
        public string Code { get; init; } = code;
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Email).NotEmpty().MaximumLength(256).EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MaximumLength(256).MinimumLength(6).Equal(q => q.ConfirmPassword);
            RuleFor(x => x.Code).NotEmpty();
        }
    }

    public class Handler(IMediator mediator, IUserRepository userRepository, ILogger<Handler> logger) : IRequestHandler<Command, Result<Empty>>
    {
        public async Task<Result<Empty>> Handle(Command request, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Params: Email={request.Email}, Code={request.Code}");

            // Find the user
            AppUser? user = await mediator.Send(new GetUserByEmail.Query(request.Email));
            if (user is null) return Result.Failed<Empty>("User does not exist");

            // Reset the password
            string token = request.Code.Contains('%') ? WebUtility.UrlDecode(request.Code) : request.Code;
            Result<Empty> result = await userRepository.ResetPasswordAsync(user, token, request.Password);
            if (result.Failed)
            {
                return Result.Failed<Empty>(result.Message);
            }

            return Result.Success<Empty>();
        }
    }
}
