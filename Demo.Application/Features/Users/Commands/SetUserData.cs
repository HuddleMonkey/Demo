using Demo.Application.Features.Content.Library.Queries;
using Demo.Application.Features.Teams.Queries;
using Demo.Application.Features.Users.Models;
using Demo.Application.Features.Users.Queries;

namespace Demo.Application.Features.Users.Commands;

/// <summary>
/// Sets additonal user data based on the include flags
/// </summary>
public class SetUserData
{
    /// <summary>
    /// Command
    /// </summary>
    /// <param name="user">User to set</param>
    /// <param name="organizationOrProviderId">Optional id of the organization or provider. If set and include flags are set, will link user to an organization/provider.</param>
    /// <param name="include">Properties to include in the results (none by default)</param>
    public class Command(AppUser? user, long organizationOrProviderId, IncludeUserProperties include = IncludeUserProperties.None) : BaseRequest, IRequest<Result<Empty>>
    {
        /// <summary>
        /// User to set
        /// </summary>
        public AppUser? User { get; init; } = user;

        /// <summary>
        /// Optional id of the organization or provider. If set and include flags are set, will link user to an organization/provider.
        /// </summary>
        public long OrganizationOrProviderId { get; init; } = organizationOrProviderId;

        /// <summary>
        /// Properties to include in the results (none by default)
        /// </summary>
        public IncludeUserProperties Include { get; init; } = include;
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.CurrentUser).NotNull();
        }
    }

    public class Handler(IMediator mediator, ILogger<Handler> logger) : IRequestHandler<Command, Result<Empty>>
    {
        public async Task<Result<Empty>> Handle(Command request, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Params: UserId={request.User?.Id}, OrganizationOrProviderId={request.OrganizationOrProviderId}, Include={request.Include}");

            if (request.User is not null)
            {
                // Teams
                if (request.Include.HasFlag(IncludeUserProperties.Teams))
                {
                    request.User.Teams = await mediator.Send(new GetUserTeams.Query(request.User.Id, request.OrganizationOrProviderId, IncludeTeamProperties.Leaders));
                }

                // Organization User
                if (request.Include.HasFlag(IncludeUserProperties.OrganizationUser) && request.OrganizationOrProviderId > 0)
                {
                    request.User.OrganizationUser = await mediator.Send(new GetOrganizationUser.Query(request.OrganizationOrProviderId, request.User.Id));
                }

                // Provider User
                if (request.Include.HasFlag(IncludeUserProperties.ProviderUser) && request.OrganizationOrProviderId > 0)
                {
                    request.User.ProviderUser = await mediator.Send(new GetProviderUser.Query(request.OrganizationOrProviderId, request.User.Id));
                }
            }

            return Result.Success<Empty>();
        }
    }
}
