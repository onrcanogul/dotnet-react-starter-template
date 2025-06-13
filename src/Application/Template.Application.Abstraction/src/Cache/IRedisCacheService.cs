namespace Template.Application.Abstraction.Cache;

public interface IRedisCacheService : ICacheService
{
    Task<IEnumerable<string>> GetKeysByPatternAsync(string pattern);
}