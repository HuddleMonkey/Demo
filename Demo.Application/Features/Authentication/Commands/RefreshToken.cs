using Demo.Application.Features.Authentication.Interfaces;
using Demo.Application.Features.Authentication.Models;
using Demo.Application.Features.Authentication.Queries;
using Demo.Application.Features.Users.Commands;
using Demo.Application.Features.Users.Interfaces;
using Demo.Application.Features.Users.Models;
using Demo.Application.Features.Users.Queries;
using Demo.Dto.Authentication;

namespace Demo.Application.Features.Authentication.Commands;

/// <summary>
/// Gets the refresh token
/// </summary>
public class RefreshToken
{
    /// <summary>
    /// Command
    /// </summary>
    /// <param name="token">Access token</param>
    /// <param name="refreshToken">Refresh token</param>
    public class Command(string token, string refreshToken) : BaseRequest, IRequest<Result<RefreshTokenResponse>>
    {
        /// <summary>
        /// Access token
        /// </summary>
        public string Token { get; init; } = token;

        /// <summary>
        /// Refresh token
        /// </summary>
        public string RefreshToken { get; init; } = refreshToken;
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Token).NotEmpty();
            RuleFor(x => x.RefreshToken).NotEmpty();
        }
    }

    public class Handler(IMediator mediator, IUserRepository userRepository, ITokenService tokenService, ILogger<Handler> logger) : IRequestHandler<Command, Result<RefreshTokenResponse>>
    {
        public async Task<Result<RefreshTokenResponse>> Handle(Command request, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Params: RefreshToken={request.RefreshToken}");

            Result<CanUserLoginResponse> canUserLogin = Result.Failed<CanUserLoginResponse>();

            // Get the user based on the expired token
            string userId = await tokenService.GetUserIdFromTokenAsync(request.Token);
            AppUser? user = await mediator.Send(new GetUserById.Query(userId));

            if (user is null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return Result.Failed<RefreshTokenResponse>("Invalid client request");
            }

            bool isAdmin = await userRepository.IsInRoleAsync(user, DemoRoles.Admin);
            if (!isAdmin)
            {
                canUserLogin = await mediator.Send(new CanUserLogin.Query(user));
                if (canUserLogin.Failed)
                {
                    return Result.Failed<RefreshTokenResponse>(canUserLogin.Message);
                }
            }

            // Save the last activity time
            user.LastActivityTime = DateTime.UtcNow;
            await mediator.Send(new UpdateUser.Command(user));

            // Generate the tokens
            IList<Claim> claims = await userRepository.GetUserClaimsAsync(user);
            string token = tokenService.GenerateToken(user, claims, canUserLogin.Data?.Organizations ?? [], isAdmin);
            string refreshToken = user.RefreshToken;

            // Build the refresh response
            RefreshTokenResponse response = new()
            {
                Token = token,
                RefreshToken = refreshToken
            };

            return Result.Success(response);
        }
    }
}
