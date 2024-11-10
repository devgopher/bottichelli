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
    private FailResult(TCommand command, ICommandContext context, string techMessage, Message? message) : base(
        command, context, message, false)
    {
    }

    public string? TechMessage { get; init; }

    public static FailResult<TCommand> Create(TCommand command,
        ICommandContext context,
        string techMessage,
        Message? message = null) =>
        new(command, context, techMessage, message);
}