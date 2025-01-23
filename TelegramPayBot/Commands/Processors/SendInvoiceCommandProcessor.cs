using System.Reflection;
using Botticelli.Client.Analytics;
using Botticelli.Framework.Commands.Processors;
using Botticelli.Framework.Commands.Validators;
using Botticelli.Framework.SendOptions;
using Botticelli.Pay.Message;
using Botticelli.Pay.Models;
using Botticelli.Pay.Utils;
using Botticelli.Shared.API.Client.Requests;
using Botticelli.Shared.ValueObjects;
using FluentValidation;

namespace TelegramPayBot.Commands.Processors;

public class SendInvoiceCommandProcessor<TReplyMarkup> : CommandProcessor<SendInvoiceCommand> where TReplyMarkup : class
{
    public SendInvoiceCommandProcessor(ILogger<SendInvoiceCommandProcessor<TReplyMarkup>> logger,
        ICommandValidator<SendInvoiceCommand> commandValidator,
        MetricsProcessor metricsProcessor,
        IValidator<Message> messageValidator)
        : base(logger, commandValidator, metricsProcessor, messageValidator)
    {
    }

    protected override Task InnerProcessContact(Message message, CancellationToken token) => Task.CompletedTask;

    protected override Task InnerProcessPoll(Message message, CancellationToken token) => Task.CompletedTask;

    protected override Task InnerProcessLocation(Message message, CancellationToken token) => Task.CompletedTask;

    protected override async Task InnerProcess(Message message, CancellationToken token)
    {
        var sendInvoiceMessageRequest = new SendMessageRequest
        {
            Message = new PayInvoiceMessage
            {
                Uid = Guid.NewGuid().ToString(),
                ChatIds = message.ChatIds,
                Body = "This is a test payments bot.\nEnjoy!",
                Invoice = new Invoice
                {
                    Title = "Test invoice (no real payment will be made)",
                    Currency = CurrencySelector.SelectCurrency("RUB"),
                    Description = "Test invoice",
                    Payload = "Test payload",
                    Prices =
                    [
                        new Price
                        {
                            Label = "Item 1",
                            Amount = 120
                        },
                        new Price
                        {
                            Label = "Item 2",
                            Amount = 150
                        }
                    ],
                    ProviderToken = "1744374395:TEST:ba361a2dee6c29728a34"
                }
            }
        };

        await Bot?.SendMessageAsync(sendInvoiceMessageRequest, token)!; // TODO: think about Bot mocks
    }
}