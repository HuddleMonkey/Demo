using Demo.Application.Features.Users.Interfaces;
using Demo.Application.Features.Users.Models;

namespace Demo.Application.Features.Users.Queries;

/// <summary>
/// Gets the organization user metadata for the user
/// </summary>
public class GetOrganizationUsersByUserId
{
    /// <summary>
    /// Query
    /// </summary>
    /// <param name="userId">Id of the user</param>
    public class Query(string userId) : BaseRequest, IRequest<List<OrganizationUser>>
    {
        /// <summary>
        /// Id of the user
        /// </summary>
        public string UserId { get; init; } = userId;
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.CurrentUser).NotNull();
        }
    }

    public class Handler(IOrganizationUserRepository organizationUserRepository, ILogger<Handler> logger) : IRequestHandler<Query, List<OrganizationUser>>
    {
        public async Task<List<OrganizationUser>> Handle(Query request, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Params: UserId={request.UserId}");

            List<OrganizationUser> users = await organizationUserRepository.GetOrganizationUsersAsync(request.UserId);

            return users;
        }
    }
}
