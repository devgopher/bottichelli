using Botticelli.Framework.Commands;

namespace Botticelli.Framework.Monads.Commands.Context;

public interface IChainCommand : ICommand
{
    public ICommandContext Context { get; init; }
}