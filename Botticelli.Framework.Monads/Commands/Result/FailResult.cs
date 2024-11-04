using Botticelli.Framework.Commands;
using Botticelli.Framework.Monads.Commands.Context;
using Botticelli.Shared.ValueObjects;

namespace Botticelli.Framework.Monads.Commands.Result;

public class FailResult<TCommand> : IResult<TCommand> where TCommand : ICommand
{
    public bool IsSuccess => false;
    public string? TechMessage { get; init; }
    public TCommand Command { get; init;}
    public ICommandContext Context { get; init;}
    public Message? Message { get; init;}
    
    public static FailResult<TCommand> Create(TCommand command,
                                              ICommandContext context,
                                              string techMessage,
                                              Message? message = null) =>
            new() { Command = command, Context = context, TechMessage = techMessage, Message = message };
}