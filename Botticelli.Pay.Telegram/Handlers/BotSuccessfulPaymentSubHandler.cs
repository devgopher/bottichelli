using Botticelli.Framework.Telegram.Handlers;
using Botticelli.Pay.Handlers;
using Botticelli.Pay.Models;
using Botticelli.Pay.Processors;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Botticelli.Pay.Telegram.Handlers;

public class BotSuccessfulPaymentSubHandler : IBotUpdateSubHandler, IPreCheckoutHandler
{
    private readonly ILogger<BotSuccessfulPaymentSubHandler> _logger;
    private readonly PayChainRunner<BotSuccessfulPaymentSubHandler, PreCheckoutQuery> _runner;

    public BotSuccessfulPaymentSubHandler(ILogger<BotSuccessfulPaymentSubHandler> logger)
    {
        _logger = logger;
        _runner = new PayChainRunner<BotSuccessfulPaymentSubHandler, PreCheckoutQuery>();
    }

    public BotSuccessfulPaymentSubHandler(ILogger<BotSuccessfulPaymentSubHandler> logger,
        PayChainRunner<BotSuccessfulPaymentSubHandler, PreCheckoutQuery> runner)
    {
        _logger = logger;
        _runner = runner;
    }

    public async Task Process(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Message?.SuccessfulPayment is null)
            return;
    }
}