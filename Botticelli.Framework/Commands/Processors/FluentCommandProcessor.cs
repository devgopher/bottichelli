using System.Text.Json;
using Botticelli.Bot.Interfaces.Processors;
using Botticelli.Bot.Utils;
using Botticelli.Client.Analytics;
using Botticelli.Framework.Commands.Validators;
using Botticelli.Interfaces;
using Botticelli.Shared.API.Client.Requests;
using Botticelli.Shared.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Botticelli.Framework.Commands.Processors;

public abstract class FluentCommandProcessor<TCommand>(
    ILogger logger,
    MetricsProcessor metricsProcessor,
    ICommandValidator<TCommand> commandValidator,
    IBot bot)
    : ICommandProcessor
    where TCommand : class, IFluentCommand
{
    protected IBot Bot = bot;
    public string CommandText { get; init; }

    public async Task ProcessAsync(Message message, CancellationToken token)
    {
        logger.LogDebug("{processorName}.ProcessAsync() : processing a message {messageUid}: {message}",
            nameof(FluentCommandProcessor<TCommand>), message.Uid, JsonSerializer.Serialize(message));

        if (!CheckCommand(message))
        {
            logger.LogDebug("{processorName}.ProcessAsync() : processing a message {messageUid}: failed",
                nameof(FluentCommandProcessor<TCommand>), message.Uid);
            return;
        }

        if (await commandValidator.Validate(message))
        {
            logger.LogDebug("{processorName}.ProcessAsync() : processing a message {messageUid}: command is valid",
                nameof(FluentCommandProcessor<TCommand>), message.Uid);
            SendMetric();
            await InnerProcess(message, token);
        }
        else
        {
            var errMessageRequest = new SendMessageRequest
            {
                Message =
                {
                    Body = commandValidator.Help()
                }
            };

            logger.LogDebug("{processorName}.ProcessAsync() : processing a message {messageUid}: command is NOT valid!",
                nameof(FluentCommandProcessor<TCommand>),
                message.Uid);

            await Bot.SendMessageAsync(errMessageRequest, token);
        }
    }

    public void SetBot(IBot bot)
        => Bot = bot;

    public void SetServiceProvider(IServiceProvider sp)
    {
    }

    private bool CheckCommand(Message message) =>
        message.Body?.ToLowerInvariant().Trim() == CommandText.ToLowerInvariant().Trim();


    private void SendMetric()
    {
        logger.LogDebug("{processorName}.SendMetric() : sending a metric...", nameof(FluentCommandProcessor<TCommand>));
        metricsProcessor.Process($"{GetType().Name.Replace("Processor", string.Empty)}Command",
            BotDataUtils.GetBotId()!);
    }


    protected abstract Task InnerProcess(Message message, CancellationToken token);
}