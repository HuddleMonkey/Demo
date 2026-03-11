using Demo.Application.Features.Organizations.Models;

namespace Demo.Application.Features.Organizations.Queries;

/// <summary>
/// Get today's date based on the organization's time zone settings
/// </summary>
public class GetTimeZoneSettings
{
    /// <summary>
    /// Query
    /// </summary>
    /// <param name="organizationId">Id of the organization</param>
    public class Query(long organizationId) : BaseRequest, IRequest<TimeZoneSettings>
    {
        /// <summary>
        /// Id of the organization
        /// </summary>
        public long OrganizationId { get; init; } = organizationId;
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
        }
    }

    public class Handler(IMediator mediator, ILogger<Handler> logger) : IRequestHandler<Query, TimeZoneSettings>
    {
        public async Task<TimeZoneSettings> Handle(Query request, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Params: organizationId={request.OrganizationId}");

            Organization? organization = await mediator.Send(new GetOrganization.Query(request.OrganizationId));
            TimeZoneSettings settings = new()
            {
                Today = DateUtils.ConvertDateFromUtc(DateTime.UtcNow, organization?.TimeZone).Date,
                TimeZone = organization?.TimeZone
            };

            return settings;
        }
    }
}
