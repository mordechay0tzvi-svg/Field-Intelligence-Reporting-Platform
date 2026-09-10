using Models;
namespace Pipeline;
public interface IElasticsearchPipline
{
    Task<List<FieldReport>> SearchAsync(string query);
    Task<List<FieldReport>> GetReportsAsync(string? theater = null,string? sector = null, string? location = null);
    Task<List<FieldReport>> GetReportsByPriorityAsync(string[]? priorities,DateTime? from,DateTime? to);
    Task<List<FieldReport>> SearchCombinedAsync(string? text = null,string? theater = null,string? sector = null,string? location = null,string[]? priorities = null,string? reportType = null,DateTime? from = null,DateTime? to = null);
    Task<Dictionary<string, int>> GetStatsForFieldAsync(string fieldName);
    Task<FieldReport?> GetBySubjetIdAsync(string subjectId);
}