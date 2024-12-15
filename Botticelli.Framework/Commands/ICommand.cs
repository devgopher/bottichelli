namespace Botticelli.Framework.Commands;

public interface ICommand
{
    Guid Id { get; }
    string CommandName => GetType().Name.Replace("Command", string.Empty);
}