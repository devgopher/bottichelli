namespace Botticelli.Analytics.Shared.Metrics;

public class MetricObject : IMetricObject
{
    public string Id => Guid.NewGuid().ToString();
    public required string? BotId { get; set; }
    public DateTime Timestamp { get; set; }
    public virtual required string Name { get; set; }
    public Dictionary<string, string>? AdditionalParameters { get; set; }
}