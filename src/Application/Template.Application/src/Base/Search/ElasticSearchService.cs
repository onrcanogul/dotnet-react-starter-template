using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Template.Application.src.Abstraction.Base.Search;

namespace Template.Application.src.Base.Search;

public class ElasticSearchService : IElasticSearchService
{
    private readonly ElasticsearchClient _client;
    private readonly ILogger<ElasticSearchService> _logger;

    public ElasticSearchService(ILogger<ElasticSearchService> logger, IConfiguration configuration)
    {
        _logger = logger;
        var settings = new ElasticsearchClientSettings(new Uri(configuration["ElasticSearch:Url"]!))
            .DefaultIndex(configuration["ElasticSearch:DefaultIndex"]!);
        _client = new ElasticsearchClient(settings);
    }
    
    
    public async Task IndexAsync<T>(string indexName, T document) where T : class
    {
        var response = await _client.IndexAsync(document, i => i.Index(indexName));
        if(!response.IsValidResponse)
            _logger.LogError($"Error in index: {indexName}");
    }

    public async Task<List<T>> SearchAsync<T>(string indexName, string keyword, string field) where T : class
    {
        var response = await _client
            .SearchAsync<T>(s => s.Index(indexName)
            .Query(q => q.Match(m => m.Field(field).Query(keyword))));
        return response.Documents.ToList();
    }
    
    public async Task<List<T>> MultiFieldFilterSearchAsync<T>(string indexName, Dictionary<string, string> fieldValuePairs) 
        where T : class
    {
        var response = await _client.SearchAsync<T>(s => s
            .Index(indexName)
            .Query(q => q
                .Bool(b => b.Must(fieldValuePairs.Select(pair => new Action<QueryDescriptor<T>>(
                            q => q.Match(m => m.Field(pair.Key).Query(pair.Value)))).ToArray())))
        );

        if (!response.IsValidResponse)
        {
            _logger.LogError("Elasticsearch search failed: {Reason}", response.DebugInformation);
        }

        return response.Documents.ToList();
    }




}