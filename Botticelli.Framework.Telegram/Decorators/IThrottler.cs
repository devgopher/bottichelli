namespace Botticelli.Framework.Telegram.Decorators;

public interface IThrottler
{
    ValueTask<T> Throttle<T>(Func<Task<T>> action, CancellationToken ct);
}