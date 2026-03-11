using Demo.Application.Features.Organizations.Models;
using Demo.Application.Features.Users.Models;
using Demo.Application.Features.Users.Queries;

namespace Demo.Application.Features.Organizations.Queries;

/// <summary>
/// Gets the organizations that are in good standing and the user is active in
/// </summary>
public class GetValidOrganizationsForUser
{
    /// <summary>
    /// Query
    /// </summary>
    /// <param name="userId">Id of the user to get valid organizations for</param>
    /// <param name="include">Properties to include in the results (none by default)</param>
    public class Query(string userId, IncludeOrganizationProperties include = IncludeOrganizationProperties.None) : BaseRequest, IRequest<Result<List<Organization>>>
    {
        /// <summary>
        /// Id of the user to get valid organizations for
        /// </summary>
        public string UserId { get; init; } = userId;

        /// <summary>
        /// Properties to include in the results (none by default)
        /// </summary>
        public IncludeOrganizationProperties Include { get; init; } = include;
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.CurrentUser).NotNull();
        }
    }

    public class Handler(IMediator mediator, ILogger<Handler> logger) : IRequestHandler<Query, Result<List<Organization>>>
    {
        public async Task<Result<List<Organization>>> Handle(Query request, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Params: userId={request.UserId}, include={request.Include}");

            List<Organization> organizations = [];

            // Verify that the user is active in any organization
            List<OrganizationUser> organizationUsers = await mediator.Send(new GetOrganizationUsersByUserId.Query(request.UserId));
            if (!organizationUsers.Any(u => u.Status == UserStatus.Active))
            {
                return Result.Failed<List<Organization>>("User account is not active");
            }

            // Check to see if any of the organizations they are associated with are in good standing
            List<long> organizationIds = [.. organizationUsers.Where(u => u.Status == UserStatus.Active).Select(u => u.OrganizationId)];
            Result<List<Organization>> resultsOrganizations = await mediator.Send(new GetOrganizationsById.Query(organizationIds, request.Include));
            organizations = resultsOrganizations.Data ?? [];
            organizations = [.. organizations.Where(o => o.Status == OrganizationStatus.Active || (o.Status == OrganizationStatus.Suspended && o.OwnerId == request.UserId))];

            if (!organizations.Any())
            {
                return Result.Failed<List<Organization>>("No valid organizations are available for your account. Possible reasons include the organization account has not yet been activated, it has been deleted, or the account has been suspended.");
            }

            return Result.Success(organizations);
        }
    }
}
