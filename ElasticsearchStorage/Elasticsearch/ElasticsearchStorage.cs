using Models;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
namespace Elasticsearch;
public interface IReportStorage
{
    Task SaveAsync(FieldReport report);
    Task CreateIndexAsync();
}
public class ReportStorage : IReportStorage
{
    private readonly ILogger<ReportStorage> _logger;
    private readonly ElasticsearchClient _client;
    private readonly string _indexName;

    public ReportStorage(IConfiguration configuration, ILogger<ReportStorage> logger)
    {
        var uri = configuration["Elasticsearch:Uri"];
        _indexName = configuration["Elasticsearch:IndexName"] ?? "field-reports";
        var settings = new ElasticsearchClientSettings(new Uri(uri!));
        _client = new ElasticsearchClient(settings);
        _logger = logger;
    }

    public async Task SaveAsync(FieldReport report)
    {
        var response = await _client.IndexAsync(report,i => i.Index(_indexName).Id(report.ReportId));
        if (!response.IsValidResponse)
        {
           _logger.LogWarning($"Failed to index report {report.ReportId}");
        }
    }
    public async Task CreateIndexAsync()
    {
        var exists = await _client.Indices.ExistsAsync(_indexName);
        if (exists.Exists)
        {
            _logger.LogInformation("Index exists");
            return;
        }
        var response = await _client.Indices.CreateAsync(
            _indexName,
            c => c
                .Mappings(m => m
                    .Properties<FieldReport>(p => p
                        .Keyword(x => x.ReportId)
                        .Keyword(x => x.SubjectId)
                        .Keyword(x => x.ReportType)
                        .Keyword(x => x.Priority)
                        .Text(x => x.Message)
                        .Date(x => x.Timestamp)
                        .Date(x => x.ProcessedAt)
                    )
                )
        );
        _logger.LogInformation("Index created");
        if (!response.IsValidResponse)
        {
            _logger.LogWarning( $"Failed to create index {_indexName}. " + $"DebugInformation: {response.DebugInformation}");
        }
    }
}
