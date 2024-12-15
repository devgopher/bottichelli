namespace Botticelli.Framework.Commands;

public interface IFluentCommand : ICommand
{
    static string? Command { get; }
}