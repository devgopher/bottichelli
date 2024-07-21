using Botticelli.Client.Analytics;
using Botticelli.Framework.Commands.Processors;
using Botticelli.Framework.Commands.Validators;
using Botticelli.Shared.API.Client.Requests;
using Botticelli.Shared.ValueObjects;

namespace TelegramCommandChainSample.Commands.CommandProcessors;

public class GetSurnameCommandProcessor : WaitForClientResponseCommandChainProcessor<SayHelloCommand>
{
    public GetSurnameCommandProcessor(ILogger<CommandChainProcessor<SayHelloCommand>> logger,
                                      ICommandValidator<SayHelloCommand> validator,
                                      MetricsProcessor metricsProcessor) : base(logger, validator, metricsProcessor)
    {
    }

    protected override async Task InnerProcess(Message message, string args, CancellationToken token)
    {
        var responseMessage = new Message
        {
            ChatIdInnerIdLinks = message.ChatIdInnerIdLinks,
            ChatIds = message.ChatIds,
            Subject = string.Empty,
            Body = "What's your surname?"
        };

        await Bot.SendMessageAsync(new SendMessageRequest
        {
            Message = responseMessage
        }, token);
    }
}