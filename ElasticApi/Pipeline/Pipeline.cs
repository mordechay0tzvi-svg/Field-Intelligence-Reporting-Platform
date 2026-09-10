using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Models;
namespace Pipeline;
public class ElasticsearchPipline : IElasticsearchPipline 
{
    private readonly ElasticsearchClient _client;
    private readonly ILogger<ElasticsearchPipline> _logger;
    public ElasticsearchPipline(ElasticsearchClient client, ILogger<ElasticsearchPipline> logger)
    {
        _client = client;
        _logger = logger;
    }
    public async Task<List<FieldReport>> SearchAsync(string query)
    {
        var response = await _client.SearchAsync<FieldReport>(s => s
        .Indices("reports")
        .From(0)
            .Query(q => q
                .MultiMatch(r => r
                    .Query(query)
                    .Fields(
                        Infer.Fields<FieldReport>(
                            x => x.Message,
                            x => x.Location,
                            x => x.Theater,
                            x => x.Sector,
                            x => x.Unit,
                            x => x.ReportType,
                            x => x.SourceType
                        )
                    )
                )
            )
        );
        if (response.IsValidResponse)
        {
            var reports = response.Documents.ToList();
        }
        return response.Documents.ToList();
    }

    public async Task<List<FieldReport>> GetReportsAsync(string? theater = null,string? sector = null, string? location = null)
    {
        var filters = new List<Query>();
        if (!string.IsNullOrWhiteSpace(theater))
        {
            filters.Add(new TermQuery
            {
                Field = "theater.keyword",
                Value = theater
            });
        }

        if (!string.IsNullOrWhiteSpace(sector))
        {
            filters.Add(new TermQuery
            {
                Field = "sector.keyword",
                Value = sector
            });
        }
        if (!string.IsNullOrWhiteSpace(location))
        {
            filters.Add(new TermQuery
            {
                Field = "location.keyword",
                Value = location
            });
        }
        var response = await _client.SearchAsync<FieldReport>
            (s => s.Indices("reports")
            .Query(q => q.Bool(b => b
            .Filter(filters)
            ))
        );
        if (!response.IsValidResponse)
        {
            _logger.LogWarning(response.DebugInformation);
        }
        return response.Documents.ToList();
    }
    public async Task<List<FieldReport>> GetReportsByPriorityAsync(string[]? priorities,DateTime? from,DateTime? to)
    {
        var filters = new List<Query>();
        if (priorities is { Length: > 0 })
        {
            var priorityQueries = priorities
            .Where(p =>
            p.Equals("High", StringComparison.OrdinalIgnoreCase) ||
            p.Equals("Critical", StringComparison.OrdinalIgnoreCase))
            .Select(p => (Query)new TermQuery
            {
                Field = "priority.keyword",
                Value = p
            })
            .ToList();

            if (priorityQueries.Count > 0)
            {
                filters.Add(new BoolQuery
                {
                    Should = priorityQueries,
                    MinimumShouldMatch = 1
                });
            }
        }
        if (from.HasValue)
        {
            filters.Add(new DateRangeQuery
            {
                Field = "timestamp",
                Gte = from.Value
            });
        }
        if (to.HasValue)
        {
            filters.Add(new DateRangeQuery
            {
                Field = "timestamp",
                Lte = to.Value
            });
        }
        var response = await _client.SearchAsync<FieldReport>(s => s
            .Indices("reports")
            .Query(q => q.Bool(b => b
            .Filter(filters)
            ))
        );
        if (!response.IsValidResponse)
        {
            _logger.LogWarning(response.DebugInformation);
        }
        return response.Documents.ToList();
    }
    
}