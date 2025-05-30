namespace Template.Application.Abstraction.Cache;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task<T> SetAsync<T>(string key, T value, TimeSpan? expiration = null);
    Task RemoveAsync(string key);
    Task<bool> ExistAsync(string key);
    Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);
}