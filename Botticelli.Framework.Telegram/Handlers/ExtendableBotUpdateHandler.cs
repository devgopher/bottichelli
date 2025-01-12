using System.Collections.Immutable;
using Botticelli.Shared.Utils;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace Botticelli.Framework.Telegram.Handlers;

public class ExtendableBotUpdateHandler : IBotUpdateHandler
{
    private readonly ImmutableList<IBotUpdateHandler> _handlers;
    
    public ExtendableBotUpdateHandler(ImmutableList<IBotUpdateHandler> handlers)
    {
        handlers.NotNullOrEmpty();
    
        _handlers = handlers;

        foreach (var handler in handlers) 
            handler.MessageReceived += (sender, args) => MessageReceived?.Invoke(sender, args);
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken) =>
        await Task.WhenAll(_handlers.Select(h => h.HandleUpdateAsync(botClient, update, cancellationToken)));

    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source,
        CancellationToken cancellationToken) =>
        await Task.WhenAll(_handlers.Select(h => h.HandleErrorAsync(botClient, exception, source, cancellationToken)));

    public event IBotUpdateHandler.MsgReceivedEventHandler? MessageReceived;
}