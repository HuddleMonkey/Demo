using Demo.Application.Features.Users.Models;

namespace Demo.Application.Features.Users.Queries;

/// <summary>
/// Gets the users in an organization.
/// </summary>
public class GetUsersInOrganization
{
    /// <summary>
    /// Query
    /// </summary>
    /// <param name="organizationId">ID of the organization to retrieve the users of</param>
    /// <param name="includeDeletedUsers">Whether to include deleted users</param>
    public class Query(long organizationId, bool includeDeletedUsers = false) : BaseRequest, IRequest<List<AppUser>>
    {
        /// <summary>
        /// ID of the organization to retrieve the users of
        /// </summary>
        public long OrganizationId { get; init; } = organizationId;

        /// <summary>
        /// Whether to include deleted users
        /// </summary>
        public bool IncludeDeletedUsers { get; init; } = includeDeletedUsers;
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.OrganizationId).GreaterThan(0);
            RuleFor(x => x.CurrentUser).NotNull();
        }
    }

    public class Handler(IMediator mediator, ILogger<Handler> logger) : IRequestHandler<Query, List<AppUser>>
    {
        public async Task<List<AppUser>> Handle(Query request, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Params: organizationId={request.OrganizationId}");

            // Get users associated with the organization
            IncludeOrganizationUserProperties include = request.IncludeDeletedUsers ? IncludeOrganizationUserProperties.DeletedUsers : IncludeOrganizationUserProperties.None;
            List<OrganizationUser> organizationUsers = await mediator.Send(new GetOrganizationUsers.Query(request.OrganizationId, userIds: [], include));
            List<string> userIds = [.. organizationUsers.Select(o => o.UserId)];

            // Get the users and set the organization user
            List<AppUser> users = await mediator.Send(new GetUsersById.Query(userIds));
            foreach (var user in users)
            {
                user.OrganizationUser = organizationUsers.FirstOrDefault(u => u.UserId == user.Id);
            }

            return users;
        }
    }
}
