namespace Template.Application.Abstraction.Base.Cache;

public interface IRedisCacheService : ICacheService
{
    Task<IEnumerable<string>> GetKeysByPatternAsync(string pattern);
}