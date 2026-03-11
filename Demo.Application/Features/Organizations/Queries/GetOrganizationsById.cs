using Demo.Application.Features.Organizations.Commands;
using Demo.Application.Features.Organizations.Interfaces;
using Demo.Application.Features.Organizations.Models;

namespace Demo.Application.Features.Organizations.Queries;

/// <summary>
/// Gets organizations by ids
/// </summary>
public class GetOrganizationsById
{
    /// <summary>
    /// Query
    /// </summary>
    /// <param name="ids">List of organization ids to retrieve</param>
    /// <param name="include">Properties to include in the results (none by default)</param>
    public class Query(List<long> ids, IncludeOrganizationProperties include = IncludeOrganizationProperties.None) : BaseRequest, IRequest<Result<List<Organization>>>
    {
        /// <summary>
        /// List of organization ids to retrieve
        /// </summary>
        public List<long> Ids { get; init; } = ids;

        /// <summary>
        /// Properties to include in the results (none by default)
        /// </summary>
        public IncludeOrganizationProperties Include { get; init; } = include;
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Ids).NotNull();
            RuleFor(x => x.CurrentUser).NotNull();
        }
    }

    public class Handler(IMediator mediator, IOrganizationRepository organizationRepository, ILogger<Handler> logger) : IRequestHandler<Query, Result<List<Organization>>>
    {
        public async Task<Result<List<Organization>>> Handle(Query request, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Params: # of organizations={request.Ids.Count}, include={request.Include}");

            // Get the organizations
            List<Organization> organizations = await organizationRepository.GetOrganizationsAsync(request.Ids, request.Include);

            // Include any data based on the flags
            foreach (var organization in organizations)
            {
                await mediator.Send(new SetOrganizationData.Command(organization, request.Include));
            }

            return Result.Success(organizations);
        }
    }
}
