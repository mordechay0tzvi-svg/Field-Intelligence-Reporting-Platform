using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Models;
namespace Pipeline;
public class ElasticsearchPipline : IElasticsearchPipline 
{
    private readonly ElasticsearchClient _client;
    private readonly ILogger<ElasticsearchPipline> _logger;
    private readonly string _indexName;
    public ElasticsearchPipline(IConfiguration configuration,ElasticsearchClient client, ILogger<ElasticsearchPipline> logger)
    {
        _client = client;
        _logger = logger;
        _indexName = configuration["Elasticsearch:IndexName"] ?? "reports";
    }
    public async Task<List<FieldReport>> SearchAsync(string query)
    {
        var response = await _client.SearchAsync<FieldReport>(s => s
        .Indices(_indexName)
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
            (s => s.Indices(_indexName)
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
            .Indices(_indexName)
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
    public async Task<List<FieldReport>> SearchCombinedAsync(string? text = null,string? theater = null,string? sector = null,string? location = null,string[]? priorities = null,string? reportType = null,DateTime? from = null,DateTime? to = null)
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
        if (!string.IsNullOrWhiteSpace(reportType))
        {
            filters.Add(new TermQuery
            {
                Field = "reportType.keyword",
                Value = reportType
            });
        }
        if (priorities != null && priorities.Length > 0)
        {
            var priorityQueries = priorities.Where(p => !string.IsNullOrWhiteSpace(p)).Select(p => 
                (Query)new TermQuery
                {
                    Field = "priority.keyword",
                    Value = p
                }).ToList();
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
        Query query;
        if (string.IsNullOrWhiteSpace(text))
        {
            query = new BoolQuery
            {
                Filter = filters
            };
        }
        else
        {
            query = new BoolQuery
            {
                Must = new List<Query>{new MatchQuery
                {Field = "message",
                Query = text}},
                Filter = filters
            };
        }
        var response = await _client.SearchAsync<FieldReport>(s => s.Indices(_indexName).Query(query));
        if (!response.IsValidResponse)
        {
            _logger.LogWarning(response.DebugInformation);
            return new List<FieldReport>();
        }
        return response.Documents.ToList();
    }
    public async Task<Dictionary<string, int>> GetStatsForFieldAsync(string fieldName)
    {
        var response = await _client.SearchAsync<FieldReport>(s => s
            .Size(0)
            .Aggregations(a => a
            .Add("counts", agg => agg
            .Terms(t => t
            .Field(fieldName)
            .Size(10000)))));
        if (!response.IsValidResponse)
        {
            _logger.LogWarning(response.DebugInformation);
        }
        if(response.Aggregations == null)
        {
            _logger.LogWarning("if twas null");
            return new Dictionary<string, int>();
        }
        var aggregation = response.Aggregations.GetStringTerms("counts");
        if (aggregation == null)
            {
                _logger.LogWarning("Aggregation 'counts' was not found.");
            }
        if(aggregation == null)
        {
            _logger.LogWarning("if twas null");
            return new Dictionary<string, int>();
        }
        return aggregation.Buckets.ToDictionary(b => b.Key.ToString(),b => (int)b.DocCount);
    }
    public async Task<FieldReport?> GetBySubjetIdAsync(string subjectId)
    {
        var response = await _client.SearchAsync<FieldReport>
        (g=>g.Indices(_indexName)
        .Query(q => q
        .Term(t=> t
        .Field(f => f.SubjectId)
        .Value(subjectId)))
        .Size(1));
        if (!response.IsValidResponse)
        {
            return null;
        }
        return response.Documents.FirstOrDefault();
    }
}