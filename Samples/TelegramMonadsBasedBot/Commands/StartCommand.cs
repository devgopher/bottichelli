using Botticelli.Framework.Commands;

namespace TelegramMonadsBasedBot.Commands;

public class StartCommand : ICommand
{
    public Guid Id { get; }
}