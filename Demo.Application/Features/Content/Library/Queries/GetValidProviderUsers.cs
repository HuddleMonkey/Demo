using Demo.Application.Features.Content.Library.Interfaces;
using Demo.Application.Features.Content.Library.Models;

namespace Demo.Application.Features.Content.Library.Queries;

/// <summary>
/// Gets the providers the user is active in
/// </summary>
public class GetValidProviderUsers
{
    /// <summary>
    /// Query
    /// </summary>
    /// <param name="userId">User Id to get valid providers for</param>
    public class Query(string userId) : BaseRequest, IRequest<Result<List<ProviderUser>>>
    {
        /// <summary>
        /// User Id to get valid providers for
        /// </summary>
        public string UserId { get; init; } = userId;
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
        }
    }

    public class Handler(IProviderUserRepository providerUserRepository, ILogger<Handler> logger) : IRequestHandler<Query, Result<List<ProviderUser>>>
    {
        public async Task<Result<List<ProviderUser>>> Handle(Query request, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Params: userId={request.UserId}");

            List<ProviderUser> providerUsers = await providerUserRepository.GetProviderUsersAsync(request.UserId);
            providerUsers = [.. providerUsers.Where(u => u.Status == UserStatus.Active)];

            return Result.Success(providerUsers);
        }
    }
}
