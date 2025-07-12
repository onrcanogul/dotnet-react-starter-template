using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Text.Json;
using Template.Application.Abstraction.Cache;

namespace Template.Application.Cache;

/// <summary>
/// Provides distributed caching functionality using Redis via IDistributedCache and StackExchange.Redis.
/// Suitable for multi-server environments and persistent caching.
/// </summary>
public class RedisCacheService : IRedisCacheService
{
    private readonly IDistributedCache _distributedCache;
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _database;
    private readonly JsonSerializerOptions _serializerOptions;

    public RedisCacheService(IDistributedCache distributedCache, IConnectionMultiplexer redis)
    {
        _distributedCache = distributedCache;
        _redis = redis;
        _database = redis.GetDatabase();
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    /// <summary>
    /// Retrieves a value from Redis cache by the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the cached object.</typeparam>
    /// <param name="key">The key of the cache entry.</param>
    /// <returns>The cached object if found; otherwise, null.</returns>
    public async Task<T?> GetAsync<T>(string key)
    {
        var cached = await _distributedCache.GetStringAsync(key);
        return cached is null ? default : JsonSerializer.Deserialize<T>(cached, _serializerOptions);
    }

    /// <summary>
    /// Stores a value in Redis cache with an optional expiration time.
    /// </summary>
    /// <typeparam name="T">The type of the object to cache.</typeparam>
    /// <param name="key">The key of the cache entry.</param>
    /// <param name="value">The value to cache.</param>
    /// <param name="expiration">Optional expiration duration.</param>
    /// <returns>The value that was cached.</returns>
    public async Task<T> SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        var json = JsonSerializer.Serialize(value, _serializerOptions);
        var options = new DistributedCacheEntryOptions();
        if (expiration.HasValue)
            options.SetAbsoluteExpiration(expiration.Value);

        await _distributedCache.SetStringAsync(key, json, options);
        return value;
    }

    /// <summary>
    /// Removes an entry from Redis cache by key.
    /// </summary>
    /// <param name="key">The key of the cache entry to remove.</param>
    public async Task RemoveAsync(string key)
    {
        await _distributedCache.RemoveAsync(key);
    }

    /// <summary>
    /// Checks whether a cache entry exists for the given key.
    /// </summary>
    /// <param name="key">The key of the cache entry.</param>
    /// <returns>True if the cache entry exists; otherwise, false.</returns>
    public async Task<bool> ExistAsync(string key)
    {
        var value = await _distributedCache.GetStringAsync(key);
        return value is not null;
    }

    /// <summary>
    /// Retrieves a value from the cache if available; otherwise, generates it using the provided factory,
    /// stores it in cache, and returns the value.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="factory">The factory method to create the value if it's not cached.</param>
    /// <param name="expiration">Optional expiration time.</param>
    /// <returns>The cached or newly created value.</returns>
    public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
    {
        var existing = await GetAsync<T>(key);
        if (existing is not null)
            return existing;

        var value = await factory();
        await SetAsync(key, value, expiration);
        return value;
    }

    /// <summary>
    /// Retrieves all cache keys matching a given pattern.
    /// This requires access to the Redis server instance and should be used with caution in production.
    /// </summary>
    /// <param name="pattern">The pattern to match keys (e.g. "user:*").</param>
    /// <returns>An enumerable of matching cache keys.</returns>
    public async Task<IEnumerable<string>> GetKeysByPatternAsync(string pattern)
    {
        var server = _redis.GetServer(_redis.GetEndPoints().First());
        var keys = server.Keys(pattern: pattern);
        return keys.Select(k => k.ToString());
    }
}
