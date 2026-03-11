using Demo.Application.Features.Users.Interfaces;
using Demo.Application.Features.Users.Models;

namespace Demo.Application.Features.Users.Queries;

/// <summary>
/// Gets the organization user
/// </summary>
public class GetOrganizationUser
{
    /// <summary>
    /// Query
    /// </summary>
    /// <param name="organizationId">Id of the organization</param>
    /// <param name="userId">Id of the user</param>
    public class Query(long organizationId, string userId) : BaseRequest, IRequest<OrganizationUser?>
    {
        /// <summary>
        /// Id of the organization
        /// </summary>
        public long OrganizationId { get; init; } = organizationId;

        /// <summary>
        /// Id of the user
        /// </summary>
        public string UserId { get; init; } = userId;
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.OrganizationId).GreaterThan(0);
            RuleFor(x => x.UserId).NotEmpty();
        }
    }

    public class Handler(IOrganizationUserRepository organizationUserRepository, ILogger<Handler> logger) : IRequestHandler<Query, OrganizationUser?>
    {
        public async Task<OrganizationUser?> Handle(Query request, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Params: organizationId={request.OrganizationId}, userId={request.UserId}");

            OrganizationUser? user = await organizationUserRepository.GetOrganizationUserAsync(request.OrganizationId, request.UserId);

            return user;
        }
    }
}
