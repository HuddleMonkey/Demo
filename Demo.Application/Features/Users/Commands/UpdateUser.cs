using Demo.Application.Features.Users.Interfaces;
using Demo.Application.Features.Users.Models;

namespace Demo.Application.Features.Users.Commands;

/// <summary>
/// Updates a user
/// </summary>
public class UpdateUser
{
    /// <summary>
    /// Command
    /// </summary>
    /// <param name="user">User to update</param>
    public class Command(AppUser user) : BaseRequest, IRequest<Result<Empty>>
    {
        /// <summary>
        /// User to update
        /// </summary>
        public AppUser User { get; init; } = user;
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.User).NotNull();
        }
    }

    public class Handler(IUserRepository userRepository, ILogger<Handler> logger) : IRequestHandler<Command, Result<Empty>>
    {
        public async Task<Result<Empty>> Handle(Command request, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Params: userId={request.User.Id}");

            Result<Empty> result = await userRepository.UpdateUserAsync(request.User);
            if (result.Failed)
            {
                return Result.Failed<Empty>(result.Message);
            }

            return Result.Success<Empty>();
        }
    }
}
