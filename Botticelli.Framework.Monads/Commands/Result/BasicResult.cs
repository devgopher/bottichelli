using Botticelli.Framework.Commands;

namespace Botticelli.Framework.Monads.Commands.Result;

public class BasicResult<TCommand> : IResult<TCommand>
    where TCommand : ICommand
{
    protected BasicResult(TCommand command, bool isSuccess)
    {
        Command = command;
        IsSuccess = isSuccess;
    }

    public bool IsSuccess { get; }
    public TCommand Command { get; }
}