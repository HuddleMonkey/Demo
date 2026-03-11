using Demo.Application.Features.Users.Commands;
using Demo.Application.Features.Users.Interfaces;
using Demo.Application.Features.Users.Models;

namespace Demo.Application.Features.Users.Queries;

/// <summary>
/// Gets a user by Id
/// </summary>
public class GetUserById
{
    /// <summary>
    /// Query
    /// </summary>
    /// <param name="userId">Id of the user to find</param>
    /// <param name="organizationOrProviderId">Optional id of the organization or provider. If set and include flags are set, will link user to an organization/provider.</param>
    /// <param name="include">Properties to include in the results (none by default)</param>
    public class Query(string userId, long organizationOrProviderId = 0, IncludeUserProperties include = IncludeUserProperties.None) : BaseRequest, IRequest<AppUser?>
    {
        /// <summary>
        /// Id of the user to find
        /// </summary>
        public string UserId { get; init; } = userId;

        /// <summary>
        /// Optional id of the organization or provider. If set and include flags are set, will link user to an organization/provider.
        /// </summary>
        public long OrganizationOrProviderId { get; init; } = organizationOrProviderId;

        /// <summary>
        /// Properties to include in the results (none by default)
        /// </summary>
        public IncludeUserProperties Include { get; init; } = include;
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
        }
    }

    public class Handler(IMediator mediator, IUserRepository userRepository, ILogger<Handler> logger) : IRequestHandler<Query, AppUser?>
    {
        public async Task<AppUser?> Handle(Query request, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Params: userId={request.UserId}, OrganizationOrProviderId={request.OrganizationOrProviderId}, include={request.Include}");

            AppUser? user = await userRepository.GetUserByIdAsync(request.UserId);
            await mediator.Send(new SetUserData.Command(user, request.OrganizationOrProviderId, request.Include));

            return user;
        }
    }
}
