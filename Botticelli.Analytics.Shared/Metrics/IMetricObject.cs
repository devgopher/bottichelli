namespace Botticelli.Analytics.Shared.Metrics;

public interface IMetricObject
{
    string Id { get; }
    string? BotId { get; set; }
    DateTime Timestamp { get; set; }
    string Name { get; set; }
    Dictionary<string, string>? AdditionalParameters { get; set; }
}