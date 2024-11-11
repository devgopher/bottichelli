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
}