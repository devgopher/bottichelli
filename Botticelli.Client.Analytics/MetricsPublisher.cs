using Botticelli.Analytics.Shared.Metrics;
using Botticelli.Client.Analytics.Settings;
using Flurl;
using Flurl.Http;

namespace Botticelli.Client.Analytics;

public class MetricsPublisher
{
    private readonly AnalyticsClientSettings _clientSettings;

    public MetricsPublisher(AnalyticsClientSettings clientSettings) => _clientSettings = clientSettings;

    public async Task Publish(IMetricObject metric, CancellationToken token)
        => await Url.Combine(_clientSettings.TargetUrl, "/metrics/receiver/ReceiveMetric")
            .SetQueryParams(metric)
            .SendAsync(HttpMethod.Get, cancellationToken: token); // polly!
}