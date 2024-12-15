namespace Botticelli.Analytics.Shared.Requests;

public class GetMetricsRequest
{
    public required string BotId { get; init; }
    public required string Name { get; init; }
    public DateTime From { get; init; }
    public DateTime To { get; init; }
}