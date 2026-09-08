using System.Text.Json.Serialization;
namespace Models;
public class FieldReport
{
    [JsonPropertyName("reportId"), JsonRequired]
    public string ReportId { get; set; } = string.Empty;
    [JsonPropertyName("timestamp"), JsonRequired]
    public DateTime Timestamp { get; set; }
    [JsonPropertyName("reportType"), JsonRequired]
    public string ReportType { get; set; } = string.Empty;
    [JsonPropertyName("sector"), JsonRequired]
    public string Sector { get; set; } = string.Empty;
    [JsonPropertyName("theater"), JsonRequired]
    public string Theater { get; set; } = string.Empty;
    [JsonPropertyName("agentId"), JsonRequired]
    public string AgentId { get; set; } = string.Empty;
    [JsonPropertyName("unit"), JsonRequired]
    public string Unit { get; set; } = string.Empty;
    [JsonPropertyName("location"), JsonRequired]
    public string Location { get; set; } = string.Empty;
    [JsonPropertyName("priority"), JsonRequired]
    public string Priority { get; set; } = string.Empty;
    [JsonPropertyName("sourceType"), JsonRequired]
    public string SourceType { get; set; } = string.Empty;
    [JsonPropertyName("message"), JsonRequired]
    public string Message { get; set; } = string.Empty;
    [JsonPropertyName("subjectId")]
    public string? SubjectId { get; set; }
    [JsonPropertyName("subjectType")]
    public string? SubjectType { get; set; }
}
