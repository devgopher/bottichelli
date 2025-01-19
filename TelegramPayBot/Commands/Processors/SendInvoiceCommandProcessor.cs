using System.Reflection;
using Botticelli.Client.Analytics;
using Botticelli.Framework.Commands.Processors;
using Botticelli.Framework.Commands.Validators;
using Botticelli.Framework.Controls.Parsers;
using Botticelli.Framework.SendOptions;
using Botticelli.Pay.Message;
using Botticelli.Pay.Models;
using Botticelli.Pay.Utils;
using Botticelli.Shared.API.Client.Requests;
using Botticelli.Shared.ValueObjects;
using FluentValidation;

namespace TelegramPayBot.Commands.Processors;

public class SendInvoiceCommandProcessor<TReplyMarkup> : CommandProcessor<InfoCommand> where TReplyMarkup : class
{
    private readonly SendOptionsBuilder<TReplyMarkup>? _options;

    public SendInvoiceCommandProcessor(ILogger<InfoCommandProcessor<TReplyMarkup>> logger,
        ICommandValidator<InfoCommand> commandValidator,
        MetricsProcessor metricsProcessor,
        ILayoutSupplier<TReplyMarkup> layoutSupplier,
        ILayoutParser layoutParser,
        IValidator<Message> messageValidator)
        : base(logger, commandValidator, metricsProcessor, messageValidator)
    {
        var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
        var responseLayout = layoutParser.ParseFromFile(Path.Combine(location, "main_layout.json"));
        var responseMarkup = layoutSupplier.GetMarkup(responseLayout);

        _options = SendOptionsBuilder<TReplyMarkup>.CreateBuilder(responseMarkup);
    }

    protected override Task InnerProcessContact(Message message, CancellationToken token) => Task.CompletedTask;

    protected override Task InnerProcessPoll(Message message, CancellationToken token) => Task.CompletedTask;

    protected override Task InnerProcessLocation(Message message, CancellationToken token) => Task.CompletedTask;

    protected override async Task InnerProcess(Message message, CancellationToken token)
    {
        var sendInvoiceMessageRequest = new SendMessageRequest
        {
            Message = new PayInvoiceMessage()
            {
                Uid = Guid.NewGuid().ToString(),
                ChatIds = message.ChatIds,
                Body = "This is a test payments bot.\nEnjoy!",
                Invoice = new Invoice
                {
                    Title = "Test invoice (no real payment will be made)",
                    Currency = CurrencySelector.SelectCurrency("USD"),
                    TotalAmount = 1550
                }
            }
        };

        await Bot?.SendMessageAsync(sendInvoiceMessageRequest, _options, token)!; // TODO: think about Bot mocks
    }
}