using Botticelli.Framework.Commands.Processors;
using Botticelli.Framework.Events;
using Botticelli.Framework.Telegram.Handlers;
using Botticelli.Pay.Message;
using Botticelli.Pay.Models;
using Botticelli.Shared.Utils;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using User = Botticelli.Shared.ValueObjects.User;

namespace Botticelli.Pay.Telegram.Handlers;

public class BotPreCheckoutHandler : IBotUpdateHandler
{
    private readonly ILogger<BotPreCheckoutHandler> _logger;
    private readonly ClientProcessorFactory _processorFactory;

    public BotPreCheckoutHandler(ILogger<BotPreCheckoutHandler> logger, ClientProcessorFactory processorFactory)
    {
        _logger = logger;
        _processorFactory = processorFactory;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    { 
        try
        {
            update.NotNull();
            update.PreCheckoutQuery.NotNull();
            update.PreCheckoutQuery!.InvoicePayload.NotNullOrEmpty();
            
            _logger.LogDebug($"{nameof(HandleUpdateAsync)}() started...");
            var message = new PayPreCheckoutMessage
            {
                Type = Shared.ValueObjects.Message.MessageType.Extended,
                PreCheckoutQuery = new PreCheckoutQuery
                {
                    Id = update.PreCheckoutQuery.Id,
                    From = new User
                    {
                        Id = update.PreCheckoutQuery.From.Id.ToString(),
                        Name = update.PreCheckoutQuery.From.FirstName,
                        Surname = update.PreCheckoutQuery.From.LastName,
                        Info = string.Empty,
                        NickName = update.PreCheckoutQuery.From.Username,
                        IsBot = update.PreCheckoutQuery.From.IsBot
                    },
                    Currency = null,
                    TotalAmount = 0,
                    InvoicePayload = null
                }
            };
            
            MessageReceived?.Invoke(this, new MessageReceivedBotEventArgs
            {
                Message = message
            });
            
            await Process(message, cancellationToken);
            
            _logger.LogDebug($"{nameof(HandleUpdateAsync)}() finished...");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(HandleUpdateAsync)}() error");
        }
    }

    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source,
        CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    /// <summary>
    ///     Processes requests
    /// </summary>
    /// <param name="request"></param>
    /// <param name="token"></param>
    protected async Task Process(Shared.ValueObjects.Message request, CancellationToken token)
    {
        _logger.LogDebug($"{nameof(Process)}({request.Uid}) started...");

        if (token is { CanBeCanceled: true, IsCancellationRequested: true })
            return;

        var clientNonChainedTasks = _processorFactory
            .GetProcessors()
            .Select(p => p.ProcessAsync(request, token));

        var clientChainedTasks = _processorFactory
            .GetCommandChainProcessors()
            .Select(p => p.ProcessAsync(request, token));

        var clientTasks = clientNonChainedTasks.Concat(clientChainedTasks).ToArray();

        await Parallel.ForEachAsync(clientTasks, token, async (t, ct) => await t.WaitAsync(ct));

        _logger.LogDebug($"{nameof(Process)}({request.Uid}) finished...");
    }
    
    public event IBotUpdateHandler.MsgReceivedEventHandler? MessageReceived;
}