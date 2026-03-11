using Demo.Application.Features.Users.Interfaces;
using Demo.Application.Features.Users.Models;

namespace Demo.Application.Features.Users.Queries;

/// <summary>
/// Gets users by Ids
/// </summary>
public class GetUsersById
{
    /// <summary>
    /// Query
    /// </summary>
    /// <param name="userIds">Ids of the users to find</param>
    public class Query(List<string> userIds) : BaseRequest, IRequest<List<AppUser>>
    {
        /// <summary>
        /// Ids of the users to find
        /// </summary>
        public List<string> UserIds { get; init; } = userIds;
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.UserIds).NotNull();
            RuleFor(x => x.CurrentUser).NotNull();
        }
    }

    public class Handler(IUserRepository userRepository, ILogger<Handler> logger) : IRequestHandler<Query, List<AppUser>>
    {
        public async Task<List<AppUser>> Handle(Query request, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Params: # of userIds={request.UserIds.Count}");

            List<AppUser> users = await userRepository.GetUsersAsync(request.UserIds);

            return users;
        }
    }
}
