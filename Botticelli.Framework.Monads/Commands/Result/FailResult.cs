using Botticelli.Framework.Commands;
using Botticelli.Framework.Monads.Commands.Context;
using Botticelli.Shared.ValueObjects;

namespace Botticelli.Framework.Monads.Commands.Result;

/// <summary>
///     Fail result
/// </summary>
/// <typeparam name="TCommand"></typeparam>
public class FailResult<TCommand> : BasicResult<TCommand> where TCommand : ICommand
{
    public string? TechMessage { get; init; }
    public bool IsSuccess => false;
    public TCommand Command { get; init; }
    public ICommandContext Context { get; init; }
    public Message? Message { get; init; }

    public static FailResult<TCommand> Create(TCommand command,
                                              ICommandContext context,
                                              string techMessage,
                                              Message? message = null) =>
            new() {Command = command, Context = context, TechMessage = techMessage, Message = message};
}