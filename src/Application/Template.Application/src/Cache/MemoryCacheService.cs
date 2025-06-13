using Microsoft.Extensions.Caching.Memory;
using Template.Application.Abstraction.Cache;

namespace Template.Application.Cache;

/// <summary>
/// Provides caching functionality using in-memory storage (local to the current application instance).
/// Note: Not suitable for multi-server or distributed environments.
/// </summary>
public class MemoryCacheService(IMemoryCache cache) : IMemoryCacheService
{
    /// <summary>
    /// Retrieves a value from the in-memory cache using the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the cached value.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <returns>The cached value, or null if not found.</returns>
    public Task<T?> GetAsync<T>(string key)
    {
        cache.TryGetValue(key, out T? value);
        return Task.FromResult(value);
    }

    /// <summary>
    /// Sets a value in the in-memory cache with an optional expiration.
    /// </summary>
    /// <typeparam name="T">The type of the value to cache.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="value">The value to store.</param>
    /// <param name="expiration">Optional absolute expiration duration.</param>
    /// <returns>The cached value.</returns>
    public Task<T> SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        var options = new MemoryCacheEntryOptions();
        if (expiration.HasValue)
        {
            options.SetAbsoluteExpiration(expiration.Value);
        }
        cache.Set(key, value, options);
        return Task.FromResult(value);
    }

    /// <summary>
    /// Removes a value from the in-memory cache using the specified key.
    /// </summary>
    /// <param name="key">The cache key to remove.</param>
    /// <returns>A completed task.</returns>
    public Task RemoveAsync(string key)
    {
        cache.Remove(key);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Checks whether a value exists in the in-memory cache for the given key.
    /// </summary>
    /// <param name="key">The cache key to check.</param>
    /// <returns>True if the key exists in cache; otherwise, false.</returns>
    public Task<bool> ExistAsync(string key)
    {
        return Task.FromResult(cache.TryGetValue(key, out _));
    }

    /// <summary>
    /// Retrieves a value from the cache if available; otherwise, uses the factory function to generate, cache, and return it.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="factory">A function to generate the value if it's not already cached.</param>
    /// <param name="expiration">Optional absolute expiration duration.</param>
    /// <returns>The cached or newly generated value.</returns>
    public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
    {
        var existing = await GetAsync<T>(key);
        if (existing is not null)
        {
            return existing;
        }
        var value = await factory();
        await SetAsync(key, value, expiration);
        return value;
    }
}
