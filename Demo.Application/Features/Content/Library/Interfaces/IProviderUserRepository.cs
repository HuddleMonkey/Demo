using Demo.Application.Features.Content.Library.Models;

namespace Demo.Application.Features.Content.Library.Interfaces;

public interface IProviderUserRepository : IRepository<ProviderUser>
{
    /// <summary>
    /// Get the metadata for the user associated with any provider
    /// </summary>
    /// <param name="userId">Id of the user</param>
    /// <returns>List of ProviderUser</returns>
    Task<List<ProviderUser>> GetProviderUsersAsync(string userId);

    /// <summary>
    /// Gets the metadata for the provider user
    /// </summary>
    /// <param name="providerId">Id of the provider</param>
    /// <param name="userId">Id of the user</param>
    /// <returns>ProviderUser or NULL if not found</returns>
    Task<ProviderUser?> GetProviderUserAsync(long providerId, string userId);
}
