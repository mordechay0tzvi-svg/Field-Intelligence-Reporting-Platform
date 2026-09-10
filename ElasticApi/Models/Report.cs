using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace Models;
public class FieldReport
{
    [JsonPropertyName("reportId"), JsonRequired, Required(AllowEmptyStrings = false)]
    public string ReportId { get; set; } = string.Empty;
    [JsonPropertyName("timestamp"), JsonRequired, Required(AllowEmptyStrings = false)]
    public DateTime Timestamp { get; set; }
    [JsonPropertyName("reportType"), JsonRequired, Required(AllowEmptyStrings = false)]
    public string ReportType { get; set; } = string.Empty;
    [JsonPropertyName("sector"), JsonRequired, Required(AllowEmptyStrings = false)]
    public string Sector { get; set; } = string.Empty;
    [JsonPropertyName("theater"), JsonRequired, Required(AllowEmptyStrings = false)]
    public string Theater { get; set; } = string.Empty;
    [JsonPropertyName("agentId"), JsonRequired, Required(AllowEmptyStrings = false)]
    public string AgentId { get; set; } = string.Empty;
    [JsonPropertyName("unit"), JsonRequired, Required(AllowEmptyStrings = false)]
    public string Unit { get; set; } = string.Empty;
    [JsonPropertyName("location"), JsonRequired, Required(AllowEmptyStrings = false)]
    public string Location { get; set; } = string.Empty;
    [JsonPropertyName("priority"), JsonRequired, Required(AllowEmptyStrings = false)]
    public string Priority { get; set; } = string.Empty;
    [JsonPropertyName("sourceType"), JsonRequired, Required(AllowEmptyStrings = false)]
    public string SourceType { get; set; } = string.Empty;
    [JsonPropertyName("message"), JsonRequired, Required(AllowEmptyStrings = false)]
    public string Message { get; set; } = string.Empty;
    [JsonPropertyName("subjectId")]
    public string? SubjectId { get; set; }
    [JsonPropertyName("subjectType")]
    public string? SubjectType { get; set; }
    public DateTime ProcessedAt { get; set; }
}
