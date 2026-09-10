using Models;
namespace Pipeline;
public interface IElasticsearchPipline
{
    Task<List<FieldReport>> SearchAsync(string query);
    Task<List<FieldReport>> GetReportsAsync(string? theater = null,string? sector = null, string? location = null);
    Task<List<FieldReport>> GetReportsByPriorityAsync(string[]? priorities,DateTime? from,DateTime? to);
}