using Botticelli.Framework.Commands;
using Botticelli.Framework.Monads.Commands.Context;

namespace TelegramMonadsBasedBot.Commands;

public class StartCommand : IChainCommand
{
    public Guid Id { get; }
    public ICommandContext Context { get; init; } = new CommandContext();
}