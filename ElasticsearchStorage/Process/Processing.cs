using System.Text.Json;
using Consumer;
using Models;
using Microsoft.Extensions.Logging;
namespace Processing;
public class Process
{
    private readonly ILogger<Process> _logger;
    private readonly IReportConsumer _consumer;
    public Process(IReportConsumer consumer, ILogger<Process> logger)
    {
        _consumer = consumer;
        _logger = logger;
    }
    public bool Validate(FieldReport report)
    {
        if (report.SubjectId == null && report.ReportType != null)
        {
            return false;
        }
        if (report.SubjectId != null && report.ReportType == null)
        {
            return false;
        }
        if (new[] { "Low", "Medium", "High", "Critical" }.Contains(report.Priority))
        {
            return false;
        }
        if (new[]{"Observation","Movement","Meeting","Access","Communication","Logistics","Incident"}.Contains(report.ReportType))
        {
            return false;
        }
        return true;
    }
    public IEnumerable<FieldReport> FilterinfReports()
    {
        List<FieldReport> reports = new List<FieldReport>();
        while (true)
        {
            var consumed = _consumer.Consume();
            if (consumed == null)
            {
                break;
            }
            else
            {
                if (consumed.Message.Value is null)
                {
                    _logger.LogWarning("Invalid reading");
                    continue;
                }
                else
                {
                    var json = consumed.Message.Value;
                    try
                    {
                        var report = JsonSerializer.Deserialize<FieldReport>(json);
                        if (report is null)
                        {
                            _logger.LogInformation("this could never happen");
                            continue;
                        }
                        if (Validate(report) == false)
                        {
                            _logger.LogWarning("invalid feild(s)");
                            continue; 
                        }
                        report.ProcessedAt = DateTime.Now;
                        reports.Add(report);
                    }
                    catch
                    {
                        _logger.LogWarning("couldn't deserialize");
                        continue;
                    }
                }
            }
        }  
        return reports;
    }
}
