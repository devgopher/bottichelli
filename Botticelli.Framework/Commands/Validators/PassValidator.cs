using Botticelli.Shared.ValueObjects;

namespace Botticelli.Framework.Commands.Validators;

public class PassValidator<TCommand> : ICommandValidator<TCommand>
    where TCommand : ICommand
{
    public Task<bool> Validate(Message message) => Task.FromResult(true);

    public string Help() => string.Empty;
}