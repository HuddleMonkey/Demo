using Demo.Application.Features.Storage.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Demo.Application.Features.Storage.Infrastructure;

public class MemoryCacheService(IMemoryCache cache, ILogger<MemoryCacheService> logger) : ICacheService
{
    /// <summary>
    /// Gets the data cached as referenced by the key, or NULL if not found
    /// </summary>
    /// <typeparam name="T">Type of class to retrieve</typeparam>
    /// <param name="key">Key used to lookup the data</param>
    /// <returns>Cached data or NULL if not found</returns>
    public T? Get<T>(string key) where T : class
    {
        logger.LogDebug($"Params: key={key}");

        bool found = cache.TryGetValue(key, out T? data);
        if (found && data is not null)
        {
            if (data is BaseEntity)
            {
                T? clone = data.Clone();
                return clone;
            }

            return data;
        }

        return null;
    }

    /// <summary>
    /// Sets the cached data with the specified key. By default, data is cached for
    /// 24 hours but can be overridden by the calling method.
    /// </summary>
    /// <typeparam name="T">Type of class to cache</typeparam>
    /// <param name="key">Key</param>
    /// <param name="data">Data to cache</param>
    /// <param name="hours">Number of hours to keep cached</param>
    public void Set<T>(string key, T data, int hours = 24)
    {
        logger.LogDebug($"Params: key={key}, hours={hours}");
        cache.Set(key, data, DateTimeOffset.Now.AddHours(hours));
    }

    /// <summary>
    /// Removes an item from the cache
    /// </summary>
    /// <param name="key">Key to remove</param>
    public void Remove(string key)
    {
        cache.Remove(key);
    }
}
