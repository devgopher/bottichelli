using Botticelli.Framework.Commands;

namespace TelegramMonadsBasedBot.Commands;

public class StopCommand : ICommand
{
    public Guid Id { get; }
}