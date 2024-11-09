using Botticelli.Framework.Commands;
using Botticelli.Framework.Monads.Commands.Context;
using Botticelli.Shared.ValueObjects;

namespace Botticelli.Framework.Monads.Commands.Result;

/// <summary>
///     Success result
/// </summary>
/// <typeparam name="TCommand"></typeparam>
public class SuccessResult<TCommand> :  BasicResult<TCommand> where TCommand : ICommand
{
    public bool IsSuccess => true;
    public TCommand Command { get; }
    public ICommandContext Context { get; }
    public Message Message { get; }

    public static FailResult<TCommand> Create(TCommand command, ICommandContext context, Message? message = null) 
        => new() {Command = command, Context = context, Message = message};
}