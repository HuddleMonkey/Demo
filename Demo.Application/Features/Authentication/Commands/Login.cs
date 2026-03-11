using Demo.Application.Features.Authentication.Interfaces;
using Demo.Application.Features.Authentication.Models;
using Demo.Application.Features.Authentication.Queries;
using Demo.Application.Features.Users.Commands;
using Demo.Application.Features.Users.Interfaces;
using Demo.Application.Features.Users.Models;
using Demo.Application.Features.Users.Queries;
using Demo.Dto.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Demo.Application.Features.Authentication.Commands;

/// <summary>
/// Logs a user into the application and returns the token and refresh token
/// </summary>
public class Login
{
    /// <summary>
    /// Command
    /// </summary>
    /// <param name="email">Email address/user name used to login</param>
    /// <param name="password">User Password</param>
    public class Command(string email, string password) : BaseRequest, IRequest<Result<LoginResponse>>
    {
        /// <summary>
        /// Email address/user name used to login
        /// </summary>
        public string Email { get; init; } = email;

        /// <summary>
        /// User Password
        /// </summary>
        public string Password { get; init; } = password;
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Email).NotEmpty().MaximumLength(256).EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MaximumLength(256);
        }
    }

    public class Handler(IMediator mediator, SignInManager<AppUser> signInManager, IUserRepository userRepository, ITokenService tokenService, ILogger<Handler> logger) : IRequestHandler<Command, Result<LoginResponse>>
    {
        public async Task<Result<LoginResponse>> Handle(Command request, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Params: {request.Email}");

            Result<CanUserLoginResponse> canUserLogin = Result.Failed<CanUserLoginResponse>();

            // Find the user by the email
            AppUser? user = await mediator.Send(new GetUserByEmail.Query(request.Email));
            if (user is not null)
            {
                // Check to see if the user is a global admin - if not, check if the user has a valid account to login with
                bool isAdmin = await userRepository.IsInRoleAsync(user, DemoRoles.Admin);
                if (!isAdmin)
                {
                    canUserLogin = await mediator.Send(new CanUserLogin.Query(user));
                    if (canUserLogin.Failed)
                    {
                        return Result.Failed<LoginResponse>(canUserLogin.Message);
                    }
                }

                // Sign in using the provided password
                SignInResult result = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
                if (result.IsLockedOut) return Result.Failed<LoginResponse>("User account locked out.");
                if (result.Succeeded)
                {
                    // Generate the tokens
                    IList<Claim> claims = await userRepository.GetUserClaimsAsync(user);
                    string token = tokenService.GenerateToken(user, claims, canUserLogin.Data?.Organizations ?? [], isAdmin);
                    string refreshToken = user.RefreshToken ?? tokenService.GenerateRefreshToken();
                    DateTime refreshTokenExpiryTime = tokenService.GetRefreshTokenExpiryTime();

                    // Update the user record with the refresh token values
                    user.RefreshToken = refreshToken;
                    user.RefreshTokenExpiryTime = refreshTokenExpiryTime;
                    user.LastActivityTime = DateTime.UtcNow;
                    _ = await mediator.Send(new UpdateUser.Command(user));

                    // Build the login response
                    LoginResponse loginResponse = new()
                    {
                        Token = token,
                        RefreshToken = refreshToken
                    };

                    return Result.Success(loginResponse);
                }
            }

            return Result.Failed<LoginResponse>("Invalid login attempt");
        }
    }
}
