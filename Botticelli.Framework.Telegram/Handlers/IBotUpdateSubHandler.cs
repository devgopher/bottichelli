using Telegram.Bot;
using Telegram.Bot.Types;

namespace Botticelli.Framework.Telegram.Handlers;

/// <summary>
///     Sub handlers for specific tasks
/// </summary>
public interface IBotUpdateSubHandler
{
    public Task Process(ITelegramBotClient botClient,
        Update update,
        CancellationToken cancellationToken);
}