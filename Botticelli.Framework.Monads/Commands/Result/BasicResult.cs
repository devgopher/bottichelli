using Botticelli.Framework.Commands;
using Botticelli.Framework.Monads.Commands.Context;
using Botticelli.Shared.ValueObjects;

namespace Botticelli.Framework.Monads.Commands.Result;

public class BasicResult<TCommand> : IResult<TCommand>
    where TCommand : ICommand
{
    protected BasicResult(TCommand command, ICommandContext commandContext, Message? message, bool isSuccess)
    {
        Command = command;
        Context = commandContext;
        Message = message;
        IsSuccess = isSuccess;
    }

    public bool IsSuccess { get; }
    public TCommand Command { get; }
    public ICommandContext Context { get; }
    public Message? Message { get; }
}