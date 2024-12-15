using Botticelli.Shared.ValueObjects;

namespace Botticelli.Framework.Commands.Validators;

public interface ICommandValidator<in TCommand>
    where TCommand : ICommand
{
    /// <summary>
    ///     Main validation procedure
    /// </summary>
    /// <returns></returns>
    public Task<bool> Validate(Message message);

    /// <summary>
    ///     A help for a concrete command
    /// </summary>
    /// <returns></returns>
    public string Help();
}