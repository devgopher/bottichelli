namespace Botticelli.Framework.Monads.Commands.Context;

public class CommandContext : ICommandContext
{
    private readonly Dictionary<string, object> _parameters = new();

    public T? Get<T>(string name) where T : class => _parameters[name] as T ?? null;

    public void Set<T>(string name, T value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));
        
        _parameters[name] = value;
    }
}