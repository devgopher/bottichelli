using System.ComponentModel.DataAnnotations;
using Botticelli.Framework.Commands;
using Botticelli.Framework.Monads.Commands.Context;
using Botticelli.Shared.ValueObjects;

namespace Botticelli.Framework.Monads.Commands.Result;

public interface IResult<out TCommand>
        where TCommand : ICommand
{
    [Required]
    public bool IsSuccess { get; }

    [Required]
    public TCommand Command { get; }

    [Required]
    public ICommandContext Context { get; }
    
    [Required]
    public Message Message { get; }
}

public class BasicResult<TCommand>(ICommand command, CommandContext commandContext, Message message) : IResult<TCommand>
        where TCommand : ICommand
{
    public bool IsSuccess { get; }
    public TCommand Command { get; } = command;
    public ICommandContext Context { get; } = commandContext;
    public Message Message { get; } = message;
}