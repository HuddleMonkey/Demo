using Demo.Application.Features.Content.Library.Interfaces;
using Demo.Application.Features.Content.Library.Models;

namespace Demo.Application.Features.Content.Library.Queries;

/// <summary>
/// Gets the provider user
/// </summary>
public class GetProviderUser
{
    /// <summary>
    /// Query
    /// </summary>
    /// <param name="providerId">Id of the provider</param>
    /// <param name="userId">Id of the user</param>
    public class Query(long providerId, string userId) : BaseRequest, IRequest<ProviderUser?>
    {
        /// <summary>
        /// Id of the provider
        /// </summary>
        public long ProviderId { get; init; } = providerId;

        /// <summary>
        /// Id of the user
        /// </summary>
        public string UserId { get; init; } = userId;
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.ProviderId).GreaterThan(0);
            RuleFor(x => x.UserId).NotEmpty();
        }
    }

    public class Handler(IProviderUserRepository providerUserRepository, ILogger<Handler> logger) : IRequestHandler<Query, ProviderUser?>
    {
        public async Task<ProviderUser?> Handle(Query request, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Params: ProviderId={request.ProviderId}, userId={request.UserId}");

            ProviderUser? user = await providerUserRepository.GetProviderUserAsync(request.ProviderId, request.UserId);

            return user;
        }
    }
}
