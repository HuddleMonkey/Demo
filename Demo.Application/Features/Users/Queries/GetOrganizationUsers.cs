using Demo.Application.Features.Users.Interfaces;
using Demo.Application.Features.Users.Models;

namespace Demo.Application.Features.Users.Queries;

/// <summary>
/// Gets the user metadata of users associated with an organization
/// </summary>
public class GetOrganizationUsers
{
    /// <summary>
    /// Query
    /// </summary>
    /// <param name="organizationId">Id of the organization</param>
    /// <param name="userIds">Optional user ids to filter by</param>
    /// <param name="include">Flags for what data to include</param>
    public class Query(long organizationId, List<string>? userIds, IncludeOrganizationUserProperties include = IncludeOrganizationUserProperties.None) : BaseRequest, IRequest<List<OrganizationUser>>
    {
        /// <summary>
        /// Id of the organization
        /// </summary>
        public long OrganizationId { get; init; } = organizationId;

        /// <summary>
        /// Optional user ids to filter by
        /// </summary>
        public List<string>? UserIds { get; init; } = userIds;

        /// <summary>
        /// Flags for what data to include
        /// </summary>
        public IncludeOrganizationUserProperties Include { get; init; } = include;
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.OrganizationId).GreaterThan(0);
            RuleFor(x => x.CurrentUser).NotNull();
        }
    }

    public class Handler(IOrganizationUserRepository organizationUserRepository, ILogger<Handler> logger) : IRequestHandler<Query, List<OrganizationUser>>
    {
        public async Task<List<OrganizationUser>> Handle(Query request, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Params: organizationId={request.OrganizationId}, # of user ids={request.UserIds?.Count}, include={request.Include}");

            List<OrganizationUser> users = request.UserIds?.Any() == true ?
                await organizationUserRepository.GetOrganizationUsersAsync(request.OrganizationId, request.UserIds, request.Include) :
                await organizationUserRepository.GetOrganizationUsersAsync(request.OrganizationId, request.Include);

            return users;
        }
    }
}
