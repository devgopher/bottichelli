namespace Botticelli.Framework.Telegram.Decorators;

public class Throttler : IThrottler
{
    private static readonly TimeSpan Delay = TimeSpan.FromSeconds(3);
    private static readonly TimeSpan MaxDeviation = TimeSpan.FromSeconds(1);
    private readonly Random _random = Random.Shared;
    private DateTime _prevDt = DateTime.MinValue;
    private readonly object _syncObj = new();


    public ValueTask<T> Throttle<T>(Func<Task<T>> action, CancellationToken ct)
    {
        lock (_syncObj)
        {
            var diff = DateTime.UtcNow - _prevDt;
            var randComponent =
                TimeSpan.FromMilliseconds(_random.Next(-MaxDeviation.Milliseconds, MaxDeviation.Milliseconds));
            var sumDelay = Delay + randComponent;

            if (diff < sumDelay) Task.Delay(sumDelay - diff, ct).WaitAsync(ct);

            _prevDt = DateTime.UtcNow;

            return new ValueTask<T>(Task.Run(action, ct));
        }
    }
}