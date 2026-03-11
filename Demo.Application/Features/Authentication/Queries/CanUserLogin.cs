using Demo.Application.Features.Authentication.Models;
using Demo.Application.Features.Content.Library.Models;
using Demo.Application.Features.Content.Library.Queries;
using Demo.Application.Features.Organizations.Models;
using Demo.Application.Features.Organizations.Queries;
using Demo.Application.Features.Users.Models;

namespace Demo.Application.Features.Authentication.Queries;

/// <summary>
/// Checks whether the user can login - either they are a member of a valid organization with an active account
/// or they are a provider
/// </summary>
public class CanUserLogin
{
    /// <summary>
    /// Query
    /// </summary>
    /// <param name="user">User to verify</param>
    public class Query(AppUser user) : BaseRequest, IRequest<Result<CanUserLoginResponse>>
    {
        /// <summary>
        /// User to verify
        /// </summary>
        public AppUser User { get; init; } = user;
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.User).NotNull();
        }
    }

    public class Handler(IMediator mediator, ILogger<Handler> logger) : IRequestHandler<Query, Result<CanUserLoginResponse>>
    {
        public async Task<Result<CanUserLoginResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Params: UserId={request.User.Id}");

            // Verify that the user is active in any organization
            Result<List<Organization>> resultOrganizations = await mediator.Send(new GetValidOrganizationsForUser.Query(request.User.Id));
            List<Organization> organizations = resultOrganizations.Data ?? [];

            // Verify if the user is active as a provider
            Result<List<ProviderUser>> resultProviders = await mediator.Send(new GetValidProviderUsers.Query(request.User.Id));
            List<ProviderUser> providers = resultProviders.Data ?? [];

            // Verify they are in organization or a provider to log in
            if (!organizations.Any() && !providers.Any())
            {
                return Result.Failed<CanUserLoginResponse>(resultOrganizations.Message);
            }

            // Build response
            CanUserLoginResponse canUserLoginResponse = new()
            {
                Organizations = organizations,
                Providers = providers
            };

            return Result.Success(canUserLoginResponse);
        }
    }
}
