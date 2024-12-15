namespace Botticelli.Framework.Telegram.Decorators;

public class Throttler : IThrottler
{
    private static readonly TimeSpan Delay = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan MaxDeviation = TimeSpan.FromSeconds(1);
    private readonly Random _random = Random.Shared;
    private DateTime _prevDt = DateTime.MinValue;


    public async ValueTask<T> Throttle<T>(Func<Task<T>> action, CancellationToken ct)
    {
        var diff = DateTime.UtcNow - _prevDt;
        var randComponent =
            TimeSpan.FromMilliseconds(_random.Next(-MaxDeviation.Milliseconds, MaxDeviation.Milliseconds));
        var sumDelay = Delay + randComponent;

        if (diff < sumDelay)
            await Task.Delay(sumDelay - diff, ct);
        _prevDt = DateTime.UtcNow;

        return await Task.Run(action, ct);
    }
}