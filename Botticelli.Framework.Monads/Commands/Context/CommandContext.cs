namespace Botticelli.Framework.Monads.Commands.Context;

/// <summary>
///     Command context for transmitting data over command context chain
/// </summary>
public class CommandContext : ICommandContext
{
    private readonly Dictionary<string, object> _parameters = new();

    public T? Get<T>(string name)
    {
        if (_parameters.TryGetValue(name, out var parameter))
            return (T?)parameter;

        return default;
    }

    public T Set<T>(string name, T value)
    {
        if (value is null) throw new ArgumentNullException(nameof(value));

        _parameters[name] = value;

        return (T)_parameters[name];
    }

    public T Transform<T>(string name, Func<T, T> func)
    {
        if (!_parameters.ContainsKey(name)) 
            throw new KeyNotFoundException(name);
        
        _parameters[name] = func((T)_parameters[name]);

        return (T)_parameters[name];
    }
}