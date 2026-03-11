using Demo.Application.Features.Organizations.Models;
using Demo.Application.Features.Payments.Interfaces;
using Demo.Application.Features.Users.Queries;

namespace Demo.Application.Features.Organizations.Commands;

/// <summary>
/// Sets additional organization data based on the include flags
/// </summary>
public class SetOrganizationData
{
    /// <summary>
    /// Command
    /// </summary>
    /// <param name="organization">Organization to set</param>
    /// <param name="include">Properties to include in the results (none by default)</param>
    public class Command(Organization organization, IncludeOrganizationProperties include = IncludeOrganizationProperties.None) : BaseRequest, IRequest<Result<Empty>>
    {
        /// <summary>
        /// Organization to set
        /// </summary>
        public Organization Organization { get; init; } = organization;

        /// <summary>
        /// Properties to include in the results (none by default)
        /// </summary>
        public IncludeOrganizationProperties Include { get; init; } = include;
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Organization).NotNull();
            RuleFor(x => x.CurrentUser).NotNull();
        }
    }

    public class Handler(IMediator mediator, IPaymentService paymentService, ILogger<Handler> logger) : IRequestHandler<Command, Result<Empty>>
    {
        public async Task<Result<Empty>> Handle(Command request, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Params: OrganizatinId={request.Organization.Id}, Include={request.Include}");

            // Users
            if (request.Include.HasFlag(IncludeOrganizationProperties.Users))
            {
                request.Organization.Users = await mediator.Send(new GetUsersInOrganization.Query(request.Organization.Id, includeDeletedUsers: request.Include.HasFlag(IncludeOrganizationProperties.DeletedUsers)));
            }

            // Subscription
            if (request.Include.HasFlag(IncludeOrganizationProperties.Subscription) && !string.IsNullOrWhiteSpace(request.Organization.StripeSubscriptionId))
            {
                request.Organization.Subscription = await paymentService.GetSubscriptionAsync(request.Organization.StripeSubscriptionId);
            }

            return Result.Success<Empty>();
        }
    }
}
