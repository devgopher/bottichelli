namespace Botticelli.Framework.Monads.Commands.Context;

/// <summary>
///     A context for monad-based commands
/// </summary>
public interface ICommandContext
{
    public T? Get<T>(string name) where T : class;
    public void Set<T>(string name, T value);
}