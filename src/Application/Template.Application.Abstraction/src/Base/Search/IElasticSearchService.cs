namespace Template.Application.src.Abstraction.Base.Search;

public interface IElasticSearchService
{
    Task IndexAsync<T>(string indexName, T document) where T : class;
    Task<List<T>> SearchAsync<T>(string indexName, string keyword, string field) where T : class;
    Task<List<T>> MultiFieldFilterSearchAsync<T>(string indexName, Dictionary<string, string> fieldValuePairs) where T : class;
}