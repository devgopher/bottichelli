using Botticelli.Framework.Commands;
using Botticelli.Framework.Monads.Commands.Context;
using Botticelli.Shared.ValueObjects;

namespace Botticelli.Framework.Monads.Commands.Result;

/// <summary>
///     Success result
/// </summary>
/// <typeparam name="TCommand"></typeparam>
public class SuccessResult<TCommand> : BasicResult<TCommand> where TCommand : ICommand
{
    private SuccessResult(TCommand command, Message? message)
        : base(command, true)
    {
    }

    public static SuccessResult<TCommand> Create(TCommand command, Message? message = null)
        => new(command, message);
}