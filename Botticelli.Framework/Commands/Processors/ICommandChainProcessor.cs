using Botticelli.Bot.Interfaces.Processors;

namespace Botticelli.Framework.Commands.Processors;

/// <summary>
/// A command chain processor means, that we've a command on input and several step to process it
/// During a processing every processor changes values in a command
/// </summary>
/// <typeparam name="TInputCommand"/>
public interface ICommandChainProcessor<TInputCommand> : ICommandChainProcessor
{
}

public interface ICommandChainProcessor : ICommandProcessor
{
    public HashSet<Guid> ChainIds { get; }

    public ICommandChainProcessor Next { get; set; }
}