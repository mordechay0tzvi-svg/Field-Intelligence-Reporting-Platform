using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
namespace Consumer;
public interface IReportConsumer
{
    ConsumeResult<string, string>? Consume();
}
public class ReportConsumer : IReportConsumer 
{
    private readonly ILogger<ReportConsumer> _logger;
    private readonly IConsumer<string, string> _consumer;
    public ReportConsumer(IConfiguration configuration, ILogger<ReportConsumer> logger)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = configuration["kafka:BootstrapServers"],
            GroupId = "catalog" + Guid.NewGuid(),
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };
        _consumer = new ConsumerBuilder<string, string>(config).Build();
        _consumer.Subscribe(configuration["Kafka:Topic"]);
        _logger = logger;
    }
    public ConsumeResult<string, string>? Consume()
    {
        try
        {
            return _consumer.Consume(TimeSpan.FromSeconds(4));
        }
        catch(Exception ex)
        {
            _logger.LogWarning($"didn't consume: {ex.Message}");
            return null;
        }
    }
}
