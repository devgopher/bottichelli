using Botticelli.Client.Analytics;
using Botticelli.Framework.Commands.Processors;
using Botticelli.Framework.Commands.Validators;
using Botticelli.Shared.API.Client.Requests;
using Botticelli.Shared.ValueObjects;
using FluentValidation;

namespace TelegramCommandChainSample.Commands.CommandProcessors;

public class GetNameCommandProcessor : WaitForClientResponseCommandChainProcessor<GetNameCommand>
{
    public GetNameCommandProcessor(ILogger<CommandChainProcessor<GetNameCommand>> logger,
        ICommandValidator<GetNameCommand> commandValidator,
        MetricsProcessor metricsProcessor,
        IValidator<Message> messageValidator) : base(logger, commandValidator, metricsProcessor, messageValidator)
    {
    }

    protected override async Task InnerProcess(Message message, string args, CancellationToken token)
    {
        var responseMessage = new Message
        {
            ChatIdInnerIdLinks = message.ChatIdInnerIdLinks,
            ChatIds = message.ChatIds,
            Subject = string.Empty,
            Body = "Hello! What's your name?"
        };

        await Bot.SendMessageAsync(new SendMessageRequest
        {
            Message = responseMessage
        }, token);
    }
}