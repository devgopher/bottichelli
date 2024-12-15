using Botticelli.Analytics.Shared.Metrics;

namespace Botticelli.Client.Analytics;

public class MetricsProcessor
{
    private readonly MetricsPublisher _publisher;

    public MetricsProcessor(MetricsPublisher publisher)
    {
        _publisher = publisher;
    }

    public void Process(string name, string? botId)
        => Task.Run(() => _publisher.Publish(new MetricObject
        {
            Name = name,
            Timestamp = DateTime.Now,
            BotId = botId
        }, CancellationToken.None));

    public void Process(IMetricObject metricObject)
        => Task.Run(() => _publisher.Publish(metricObject, CancellationToken.None));
}