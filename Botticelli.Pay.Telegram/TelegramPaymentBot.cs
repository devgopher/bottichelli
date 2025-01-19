using Botticelli.Bot.Data.Repositories;
using Botticelli.Bot.Utils.TextUtils;
using Botticelli.Client.Analytics;
using Botticelli.Framework.Telegram;
using Botticelli.Framework.Telegram.Handlers;
using Botticelli.Interfaces;
using Botticelli.Pay.Message;
using Botticelli.Shared.API.Client.Requests;
using Botticelli.Shared.API.Client.Responses;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.Payments;

namespace Botticelli.Pay.Telegram;

public class TelegramPaymentBot : TelegramBot
{
    public TelegramPaymentBot(ITelegramBotClient client, IBotUpdateHandler handler, ILogger<TelegramBot> logger,
        MetricsProcessor metrics, ITextTransformer textTransformer, IBotDataAccess data) : base(client, handler, logger,
        metrics, textTransformer, data)
    {
    }


    protected override async Task<Shared.ValueObjects.Message> AdditionalProcessing<TSendOptions>(
        Shared.ValueObjects.Message message,
        ISendOptionsBuilder<TSendOptions>? optionsBuilder, bool isUpdate, CancellationToken token, string chatId)
    {
        message = await base.AdditionalProcessing(message, optionsBuilder, isUpdate, token, chatId);

        var invoice = (message as PayInvoiceMessage)?.Invoice;
        
        if (invoice is not null)
        {

            var sentMessage = await Client.SendInvoice(chatId,
                title: invoice.Title,
                description: invoice.Description,
                currency: invoice.Currency.Demonym,
                payload: invoice.Payload,
                prices: invoice.Prices.Select(p => new LabeledPrice(p.Label, ConvertPrice(p.Amount))).ToList(), cancellationToken: token);
            
            // TODO: additional checks
        }
        
        
        return message;
    }
    
    private int ConvertPrice(decimal price) => Convert.ToInt32(price * 100);
}