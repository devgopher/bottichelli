namespace Botticelli.Framework.Monads.Commands.Context;

/// <summary>
///     Command context for transmitting data over command context chain
/// </summary>
public class CommandContext : ICommandContext
{
    private readonly Dictionary<string, object> _parameters = new();

    public T? Get<T>(string name) where T : class
    {
        return _parameters[name] as T ?? null;
    }

    public void Set<T>(string name, T value)
    {
        if (value is null) throw new ArgumentNullException(nameof(value));

        _parameters[name] = value;
    }
}