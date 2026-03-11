using Demo.Application.Features.Events.Interfaces;
using Demo.Application.Features.Events.Models;
using Demo.Dto.Events;

namespace Demo.Application.Features.Events.Queries;

/// <summary>
/// Gets the locations. If any new locations were added in the positions, add and return the full list.
/// </summary>
public class GetLocationsFromPositions
{
    /// <summary>
    /// Query
    /// </summary>
    /// <param name="organizationId">Id of the associated organization</param>
    /// <param name="positions">Positions with possible locations</param>
    public class Query(long organizationId, List<PositionDto> positions) : BaseRequest, IRequest<List<Location>>
    {
        /// <summary>
        /// Id of the associated organization
        /// </summary>
        public long OrganizationId { get; init; } = organizationId;

        /// <summary>
        /// Positions with possible locations
        /// </summary>
        public List<PositionDto> Positions { get; init; } = positions;
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.OrganizationId).GreaterThan(0);
            RuleFor(x => x.Positions).NotNull();
            RuleFor(x => x.CurrentUser).NotNull();
        }
    }

    public class Handler(ILocationRepository locationRepository, ILogger<Handler> logger) : IRequestHandler<Query, List<Location>>
    {
        public async Task<List<Location>> Handle(Query request, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Params: OrganizationId={request.OrganizationId}");

            // Get the existing locations
            List<Location> locations = await locationRepository.GetLocationsAsync(request.OrganizationId);
            List<string> existingLocationNames = [.. locations.Select(location => location.Name)];

            // Get the positions that are associated with the positions and add any new ones
            List<string> positionLocations = [.. request.Positions.Where(p => !string.IsNullOrEmpty(p.Location?.Name)).Select(p => p.Location!.Name).Distinct()];
            List<string> newLocations = [.. positionLocations.Except(existingLocationNames)];
            if (newLocations.Any())
            {
                List<Location> locationsToAdd = [];
                foreach (var location in newLocations)
                {
                    locationsToAdd.Add(new Location() { OrganizationId = request.OrganizationId, Name = location });
                }
                await locationRepository.SaveEntitiesAsync(locationsToAdd);
                locations.AddRange(locationsToAdd);
            }

            return locations;
        }
    }
}
