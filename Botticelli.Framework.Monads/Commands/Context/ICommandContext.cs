namespace Botticelli.Framework.Monads.Commands.Context;

/// <summary>
///     A context for monad-based commands
/// </summary>
public interface ICommandContext
{
    public T? Get<T>(string name);
    public T Set<T>(string name, T value);
    public T Transform<T>(string name, Func<T, T> func);
}