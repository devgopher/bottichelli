using Botticelli.Client.Analytics;
using Botticelli.Framework.Commands.Processors;
using Botticelli.Framework.Commands.Utils;
using Botticelli.Framework.Commands.Validators;
using Botticelli.Shared.API.Client.Requests;
using Botticelli.Shared.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Botticelli.Locations.Commands.CommandProcessors;

public class MapCommandProcessor<TReplyMarkup> : CommandProcessor<MapCommand>
    where TReplyMarkup : class
{
    public MapCommandProcessor(ILogger<MapCommandProcessor<TReplyMarkup>> logger,
                               ICommandValidator<MapCommand> commandValidator,
                               MetricsProcessor metricsProcessor,
                               IValidator<Message> messageValidator) 
        : base(logger, commandValidator, metricsProcessor, messageValidator)
    {
    }

    protected override async Task InnerProcess(Message message, CancellationToken token)
    {
        var request = new SendMessageRequest
        {
            Message = new Message
            {
                Uid = Guid.NewGuid().ToString(),
                ChatIds = message.ChatIds,
                Body = message.Body?.GetArguments()
            }
        };

        await Bot.SendMessageAsync(request,  token);
    }
}