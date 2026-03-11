using Demo.Application.Features.Organizations.Commands;
using Demo.Application.Features.Organizations.Interfaces;
using Demo.Application.Features.Organizations.Models;

namespace Demo.Application.Features.Organizations.Queries;

/// <summary>
/// Gets an organization by its id
/// </summary>
public class GetOrganization
{
    /// <summary>
    /// Query
    /// </summary>
    /// <param name="id">Organization id to retrieve</param>
    /// <param name="include">Properties to include in the results (none by default)</param>
    public class Query(long id, IncludeOrganizationProperties include = IncludeOrganizationProperties.None) : BaseRequest, IRequest<Organization?>
    {
        /// <summary>
        /// Organization id to retrieve
        /// </summary>
        public long Id { get; init; } = id;

        /// <summary>
        /// Properties to include in the results (none by default)
        /// </summary>
        public IncludeOrganizationProperties Include { get; init; } = include;
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }

    public class Handler(IMediator mediator, IOrganizationRepository organizationRepository, ILogger<Handler> logger) : IRequestHandler<Query, Organization?>
    {
        public async Task<Organization?> Handle(Query request, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Params: Id={request.Id}, include={request.Include}");

            Organization? organization = await organizationRepository.GetOrganizationAsync(request.Id, request.Include);
            if (organization is not null)
            {
                await mediator.Send(new SetOrganizationData.Command(organization, request.Include));
            }

            return organization;
        }
    }
}
