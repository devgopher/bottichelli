using Botticelli.Framework.Events;
using Botticelli.Framework.Telegram.Handlers;
using Botticelli.Pay.Handlers;
using Botticelli.Pay.Message;
using Botticelli.Pay.Models;
using Botticelli.Pay.Processors;
using Botticelli.Shared.Utils;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using User = Botticelli.Shared.ValueObjects.User;

namespace Botticelli.Pay.Telegram.Handlers;

public class BotPreCheckoutHandler : IBotUpdateHandler, IPreCheckoutHandler
{
    private readonly ILogger<BotPreCheckoutHandler> _logger;
    private readonly PreCheckoutChainRunner<BotPreCheckoutHandler> _runner;

    public BotPreCheckoutHandler(ILogger<BotPreCheckoutHandler> logger, PreCheckoutChainRunner<BotPreCheckoutHandler> runner)
    {
        _logger = logger;
        _runner = runner;
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
            
            
            var preCheckoutQuery = new PreCheckoutQuery
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
                Currency = update.PreCheckoutQuery.Currency,
                TotalAmount = update.PreCheckoutQuery.TotalAmount,
                InvoicePayload =update.PreCheckoutQuery.InvoicePayload,
            };
            
            var procResult = await _runner.Run(preCheckoutQuery, cancellationToken);
            
            await botClient.AnswerPreCheckoutQuery(preCheckoutQuery.Id, procResult.isSuccessful ? string.Empty : procResult.errorMessage, cancellationToken: cancellationToken);
            
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

    public event IBotUpdateHandler.MsgReceivedEventHandler? MessageReceived;
}