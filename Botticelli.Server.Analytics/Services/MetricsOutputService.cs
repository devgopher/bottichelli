using Botticelli.Analytics.Shared.Metrics;
using Botticelli.Analytics.Shared.Requests;
using Botticelli.Analytics.Shared.Responses;
using Botticelli.Server.Analytics.Cache;

namespace Botticelli.Server.Analytics.Services;

public class MetricsOutputService : IMetricsOutputService
{
    private readonly ICacheAccessor _cacheAccessor;
    private readonly MetricsReaderWriter _rw;

    public MetricsOutputService(MetricsReaderWriter rw, ICacheAccessor cacheAccessor)
    {
        _rw = rw;
        _cacheAccessor = cacheAccessor;
    }

    public async Task<GetMetricsResponse> GetMetricsAsync(GetMetricsRequest request,
        CancellationToken token)
    {
        var count = 0;

        count = _cacheAccessor.ReadCount(x =>
            x.Timestamp >= request.From && x.Timestamp <= request.To && x.BotId == request.BotId &&
            x.Name == request.Name);

        if (count == 0)
            count = await _rw.ReadCountAsync(x =>
                    x.Timestamp >= request.From && x.Timestamp <= request.To && x.BotId == request.BotId &&
                    x.Name == request.Name,
                token);


        return new GetMetricsResponse
        {
            From = request.From,
            To = request.To,
            Count = count
        };
    }

    public async Task<GetMetricsIntervalsResponse> GetMetricsForIntervalAsync(GetMetricsForIntervalsRequest request,
        CancellationToken token)
    {
        var metrics = await _rw.ReadCountForFramesAsync(request.Name,
            request.BotId,
            request.From,
            request.To,
            TimeSpan.FromSeconds(request.Interval),
            token);

        var metricsForIntervals = metrics.Select(m => new GetMetricsResponse
            {
                Count = m.count,
                From = m.dt1,
                To = m.dt2
            }).ToBlockingEnumerable()
            .ToList();

        var commonCount = metricsForIntervals.Sum(m => m.Count);

        return new GetMetricsIntervalsResponse
        {
            From = request.From,
            To = request.To,
            Count = commonCount,
            MetricsForIntervals = metricsForIntervals
        };
    }

    public async Task<IEnumerable<string>> GetMetricNamesAsync(CancellationToken token)
    {
        var result = new List<string>();
        result.AddRange(MetricNames.Names);

        var additionalMetrics = (await _rw.GetMetricNamesAsync(token)).Except(result);
        result.AddRange(additionalMetrics);

        return result;
    }
}