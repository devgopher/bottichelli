using Botticelli.Framework.Telegram.Handlers;
using Botticelli.Pay.Handlers;
using Botticelli.Pay.Models;
using Botticelli.Pay.Processors;
using Botticelli.Pay.Utils;
using Botticelli.Shared.Utils;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = Botticelli.Shared.ValueObjects.User;

namespace Botticelli.Pay.Telegram.Handlers;

public class BotPreCheckoutSubHandler : IBotUpdateSubHandler, IPreCheckoutHandler
{
    private readonly ILogger<BotPreCheckoutSubHandler> _logger;
    private readonly PayChainRunner<BotPreCheckoutSubHandler, PreCheckoutQuery> _runner;

    public BotPreCheckoutSubHandler(ILogger<BotPreCheckoutSubHandler> logger)
    {
        _logger = logger;
        _runner = new PayChainRunner<BotPreCheckoutSubHandler, PreCheckoutQuery>();
    }

    public BotPreCheckoutSubHandler(ILogger<BotPreCheckoutSubHandler> logger,
        PayChainRunner<BotPreCheckoutSubHandler, PreCheckoutQuery> runner)
    {
        _logger = logger;
        _runner = runner;
    }

    public async Task Process(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        try
        {
            update.NotNull();
            if (update.PreCheckoutQuery is null)
                return;

            update.PreCheckoutQuery!.InvoicePayload.NotNullOrEmpty();

            _logger.LogDebug($"{nameof(Process)}() started...");

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
                Currency = CurrencySelector.SelectCurrency(update.PreCheckoutQuery.Currency),
                TotalAmount = update.PreCheckoutQuery.TotalAmount,
                InvoicePayload = update.PreCheckoutQuery.InvoicePayload
            };

            var procResult = await _runner.Run(preCheckoutQuery, cancellationToken);

            await botClient.AnswerPreCheckoutQuery(preCheckoutQuery.Id,
                procResult.isSuccessful ? default : procResult.errorMessage, cancellationToken);

            _logger.LogDebug($"{nameof(Process)}() finished...");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(Process)}() error");
        }
    }
}