using Botticelli.Bot.Data.Repositories;
using Botticelli.Bot.Utils.TextUtils;
using Botticelli.Client.Analytics;
using Botticelli.Framework.Telegram;
using Botticelli.Framework.Telegram.Handlers;
using Botticelli.Interfaces;
using Botticelli.Pay.Message;
using Botticelli.Shared.API.Client.Requests;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Payments;

namespace Botticelli.Pay.Telegram;

public class TelegramPaymentBot : TelegramBot
{
    public TelegramPaymentBot(ITelegramBotClient client, 
        IBotUpdateHandler handler, ILogger<TelegramPaymentBot> logger,
        MetricsProcessor metrics, ITextTransformer textTransformer, IBotDataAccess data) : base(client, handler, logger,
        metrics, textTransformer, data)
    {
    }

    protected override async Task AdditionalProcessing<TSendOptions>(SendMessageRequest request,
        ISendOptionsBuilder<TSendOptions>? optionsBuilder, bool isUpdate, string chatId, CancellationToken token)
    {
        await base.AdditionalProcessing(request, optionsBuilder, isUpdate, chatId, token);
        
        var invoice = (request.Message as PayInvoiceMessage)?.Invoice;

        if (invoice is not null)
        {
            await Client.SendInvoice(chatId,
                title: invoice.Title,
                description: invoice.Description,
                currency: invoice.Currency.Demonym,
                payload: invoice.Payload,
                providerData: invoice.ProviderData,
                providerToken: invoice.ProviderToken,
                prices: invoice.Prices.Select(p => new LabeledPrice(p.Label, ConvertPrice(p.Amount))).ToList(),
                cancellationToken: token);
        }
    }
    
    private int ConvertPrice(decimal price) => Convert.ToInt32(price * 100);
}