using System.Collections.Concurrent;
using Botticelli.Analytics.Shared.Requests;
using Botticelli.Analytics.Shared.Responses;
using Botticelli.Server.Analytics.Models;
using Botticelli.Server.Analytics.Utils;
using Polly;

namespace Botticelli.Server.Analytics.Cache;

/// <summary>
///     Local-based memory cache with auto-cleaning
/// </summary>
public class LocalCacheAccessor : ICacheAccessor
{
    private static readonly ConcurrentDictionary<string, MetricModel> MemoryCache = new();
    private static long _maxCacheSize = 1024768;
    private static Task<PolicyResult<bool>> _cacheCleaningTask;
    private static readonly CancellationTokenSource CancellationTokenSource = new();
    private static readonly CancellationToken Token = CancellationTokenSource.Token;

    public LocalCacheAccessor(long maxCacheSize)
    {
        _maxCacheSize = maxCacheSize;
        InitCacheCleaning();
    }


    public int ReadCount(Func<MetricModel, bool> func)
        => MemoryCache
            .Values
            .Where(func)
            .Count();

    public GetMetricsIntervalsResponse GetForIntervals(GetMetricsForIntervalsRequest request)
    {
        var frames = DateTimeUtils.GetRange(request.From,
            request.To,
            TimeSpan.FromSeconds(request.Interval));

        var results = frames.Select(f => new GetMetricsResponse
        {
            From = f.dt1,
            To = f.dt2,
            Count = MemoryCache.Count(mm => mm.Value.BotId == request.BotId &&
                                            mm.Value.Name == request.Name &&
                                            mm.Value.Timestamp >= f.dt1.ToUniversalTime() &&
                                            mm.Value.Timestamp <= f.dt2.ToUniversalTime())
        }).Where(f => f.Count > 0);

        return new GetMetricsIntervalsResponse
        {
            From = request.From,
            To = request.To,
            MetricsForIntervals = results
        };
    }

    public void Set(MetricModel request)
    {
        MemoryCache[request.Id] = request;
    }

    public void Clear(DateTime until)
    {
        var toRemove = MemoryCache.Where(c => c.Value.Timestamp < until)
            .Select(kvp => kvp.Key).ToArray();

        foreach (var c in toRemove)
            MemoryCache.TryRemove(c, out _);
    }

    public void Remove(MetricModel request)
    {
        MemoryCache.TryRemove(request.Id, out _);
    }

    private static void InitCacheCleaning()
    {
        if (_cacheCleaningTask != null)
            return;

        _cacheCleaningTask = Policy.HandleResult<bool>(r => true)
            .WaitAndRetryForeverAsync(_ => TimeSpan.FromMinutes(1))
            .ExecuteAndCaptureAsync(async ct =>
            {
                var size = MemoryCache.Count;

                while (size >= _maxCacheSize)
                {
                    if (Token is { CanBeCanceled: true, IsCancellationRequested: true })
                        break;

                    var oldest = MemoryCache
                        .Values.MinBy(c => c.Timestamp);

                    if (oldest == null)
                        break;

                    MemoryCache.TryRemove(oldest.Id, out _);

                    size = MemoryCache.Count;
                }

                return true;
            }, Token);
    }
}