using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Template.Application.src.Abstraction.Base.Search;

namespace Template.Application.src.Base.Search;

/// <summary>
/// Service for interacting with Elasticsearch using the official .NET client.
/// Provides methods for indexing documents and performing flexible search queries.
/// </summary>
public class ElasticSearchService : IElasticSearchService
{
    private readonly ElasticsearchClient _client;
    private readonly ILogger<ElasticSearchService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ElasticSearchService"/> class.
    /// </summary>
    /// <param name="logger">Logger for logging Elasticsearch errors or status.</param>
    /// <param name="configuration">Configuration containing Elasticsearch connection settings.</param>
    public ElasticSearchService(ILogger<ElasticSearchService> logger, IConfiguration configuration)
    {
        _logger = logger;
        var settings = new ElasticsearchClientSettings(new Uri(configuration["ElasticSearch:Url"]!))
            .DefaultIndex(configuration["ElasticSearch:DefaultIndex"]!);
        _client = new ElasticsearchClient(settings);
    }

    /// <summary>
    /// Indexes a single document into a specified Elasticsearch index.
    /// </summary>
    /// <typeparam name="T">The type of the document.</typeparam>
    /// <param name="indexName">The target index name.</param>
    /// <param name="document">The document to be indexed.</param>
    public async Task IndexAsync<T>(string indexName, T document) where T : class
    {
        var response = await _client.IndexAsync(document, i => i.Index(indexName));
        if (!response.IsValidResponse)
            _logger.LogError($"Error in index: {indexName}");
    }

    /// <summary>
    /// Performs a keyword-based match search on a single field.
    /// </summary>
    /// <typeparam name="T">The type of documents to search.</typeparam>
    /// <param name="indexName">The index to search.</param>
    /// <param name="keyword">The search keyword.</param>
    /// <param name="field">The field name to search in.</param>
    /// <returns>List of matching documents.</returns>
    public async Task<List<T>> SearchAsync<T>(string indexName, string keyword, string field) where T : class
    {
        var response = await _client
            .SearchAsync<T>(s => s.Index(indexName)
            .Query(q => q.Match(m => m.Field(field).Query(keyword))));
        return response.Documents.ToList();
    }

    /// <summary>
    /// Performs a multi-field match search where each field has its own specific value.
    /// Each key-value pair in the dictionary represents a field and the expected match value.
    /// </summary>
    /// <typeparam name="T">The type of documents to search.</typeparam>
    /// <param name="indexName">The index to search.</param>
    /// <param name="fieldValuePairs">Dictionary containing field names and their corresponding search values.</param>
    /// <returns>List of matching documents based on all field conditions (AND logic).</returns>
    public async Task<List<T>> MultiFieldFilterSearchAsync<T>(string indexName, Dictionary<string, string> fieldValuePairs)
        where T : class
    {
        var response = await _client.SearchAsync<T>(s => s
            .Index(indexName)
            .Query(q => q
                .Bool(b => b.Must(fieldValuePairs.Select(pair =>
                    new Action<QueryDescriptor<T>>(
                        q => q.Match(m => m.Field(pair.Key).Query(pair.Value)))
                ).ToArray()))
            )
        );

        if (!response.IsValidResponse)
        {
            _logger.LogError("Elasticsearch search failed: {Reason}", response.DebugInformation);
        }

        return response.Documents.ToList();
    }
}
