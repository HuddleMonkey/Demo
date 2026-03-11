namespace Demo.Application.Features.Storage.Interfaces;

public interface ICacheService
{
    /// <summary>
    /// Gets the data cached as referenced by the key, or NULL if not found
    /// </summary>
    /// <typeparam name="T">Type of class to retrieve</typeparam>
    /// <param name="key">Key used to lookup the data</param>
    /// <returns>Cached data or NULL if not found</returns>
    T? Get<T>(string key) where T : class;

    /// <summary>
    /// Sets the cached data with the specified key. By default, data is cached for
    /// 24 hours but can be overridden by the calling method.
    /// </summary>
    /// <typeparam name="T">Type of class to cache</typeparam>
    /// <param name="key">Key</param>
    /// <param name="data">Data to cache</param>
    /// <param name="hours">Number of hours to keep cached</param>
    void Set<T>(string key, T data, int hours = 24);

    /// <summary>
    /// Removes an item from the cache
    /// </summary>
    /// <param name="key">Key to remove</param>
    void Remove(string key);
}
