using System.Text.Json;
using Consumer;
using Models;
using Validator;
using Microsoft.Extensions.Logging;
namespace Processing;
public class Process
{
    private readonly ILogger<Process> _logger;
    private readonly IReportConsumer _consumer;
    private readonly IValidating _validator;
    public Process(IReportConsumer consumer, ILogger<Process> logger, IValidating validator)
    {
        _consumer = consumer;
        _logger = logger;
        _validator = validator;
    }
    public IEnumerable<FieldReport> FilteringReports()
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
                        if (_validator.Validate(report) == false)
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
