namespace Botticelli.Framework.Commands.Validators;

public class PassValidator<TCommand> : ICommandValidator<TCommand>
    where TCommand : ICommand
{
    public ICommand Command { get; }
    public Task<bool> Validate(string args) => Task.FromResult(true);

    public string Help() => string.Empty;
}