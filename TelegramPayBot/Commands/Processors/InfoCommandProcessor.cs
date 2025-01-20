using System.Reflection;
using Botticelli.Client.Analytics;
using Botticelli.Framework.Commands.Processors;
using Botticelli.Framework.Commands.Validators;
using Botticelli.Framework.Controls.Parsers;
using Botticelli.Framework.SendOptions;
using Botticelli.Shared.API.Client.Requests;
using Botticelli.Shared.ValueObjects;
using FluentValidation;

namespace TelegramPayBot.Commands.Processors;

public class InfoCommandProcessor<TReplyMarkup> : CommandProcessor<InfoCommand> where TReplyMarkup : class
{
    public InfoCommandProcessor(ILogger<InfoCommandProcessor<TReplyMarkup>> logger,
        ICommandValidator<InfoCommand> commandValidator,
        MetricsProcessor metricsProcessor,
        ILayoutSupplier<TReplyMarkup> layoutSupplier,
        ILayoutParser layoutParser,
        IValidator<Message> messageValidator)
        : base(logger, commandValidator, metricsProcessor, messageValidator)
    {
    }

    protected override Task InnerProcessContact(Message message, CancellationToken token) => Task.CompletedTask;

    protected override Task InnerProcessPoll(Message message, CancellationToken token) => Task.CompletedTask;

    protected override Task InnerProcessLocation(Message message, CancellationToken token) => Task.CompletedTask;

    protected override async Task InnerProcess(Message message, CancellationToken token)
    {
        var greetingMessageRequest = new SendMessageRequest
        {
            Message = new Message
            {
                Uid = Guid.NewGuid().ToString(),
                ChatIds = message.ChatIds,
                Body = "This is a test payment bot.\nEnjoy!"
            }
        };

        await Bot?.SendMessageAsync(greetingMessageRequest, token)!; // TODO: think about Bot mocks
    }
}