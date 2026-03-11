using Demo.Application.Features.Content.Library.Interfaces;
using Demo.Application.Features.Content.Library.Models;

namespace Demo.Application.Features.Content.Library.Infrastructure;

public class SqlProviderUserRepository(DemoDbContext context, ILogger<SqlProviderUserRepository> repoLogger) : Repository<ProviderUser>(context, repoLogger), IProviderUserRepository
{
    /// <summary>
    /// Get the metadata for the user associated with any provider
    /// </summary>
    /// <param name="userId">Id of the user</param>
    /// <returns>List of ProviderUser</returns>
    public async Task<List<ProviderUser>> GetProviderUsersAsync(string userId)
    {
        logger.LogDebug($"Params: userId={userId}");

        List<ProviderUser> users = await context.ProviderUsers
            .Where(o => o.UserId == userId)
            .ToListAsync();

        return users;
    }

    /// <summary>
    /// Gets the metadata for the provider user
    /// </summary>
    /// <param name="providerId">Id of the provider</param>
    /// <param name="userId">Id of the user</param>
    /// <returns>ProviderUser or NULL if not found</returns>
    public async Task<ProviderUser?> GetProviderUserAsync(long providerId, string userId)
    {
        logger.LogDebug($"Params: providerId={providerId}, userId={userId}");

        ProviderUser? user = await context.ProviderUsers.FirstOrDefaultAsync(u => u.ProviderId == providerId && u.UserId == userId);

        return user;
    }
}
