using Botticelli.Analytics.Shared.Metrics;
using Botticelli.Analytics.Shared.Requests;

namespace Botticelli.Server.Analytics.Services;

public class MetricsInputService : IMetricsInputService
{
    private readonly MetricsReaderWriter _rw;

    public MetricsInputService(MetricsReaderWriter rw)
    {
        _rw = rw;
    }

    public async Task PushMetricAsync(PushMetricRequest<IMetricObject> request, CancellationToken token)
        => await _rw.WriteAsync(request.Object, token);
}