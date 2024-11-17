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
        ICommandValidator<TCommand> validator,
        IBot bot)
        : ICommandProcessor
        where TCommand : class, IFluentCommand
{
    private readonly ILogger _logger = logger;
    protected IBot Bot = bot;
    public string CommandText { get; init; }

    public async Task ProcessAsync(Message message, CancellationToken token)
    {
        if (!CheckCommand(message)) return;

        if (await validator.Validate(message.Body))
        {
            SendMetric();
            await InnerProcess(message, token);
        }
        else
        {
            var errMessageRequest = new SendMessageRequest
            {
                Message =
                {
                    Body = validator.Help()
                }
            };

            await Bot.SendMessageAsync(errMessageRequest, token);
        }
    }

    public void SetBot(IBot bot)
        => Bot = bot;

    public void SetServiceProvider(IServiceProvider sp)
    {
    }

    private bool CheckCommand(Message message) => message.Body?.ToLowerInvariant().Trim() == CommandText.ToLowerInvariant().Trim();


    private void SendMetric(string metricName) => metricsProcessor.Process(metricName, BotDataUtils.GetBotId()!);

    private void SendMetric() => metricsProcessor.Process($"{GetType().Name.Replace("Processor", string.Empty)}Command", BotDataUtils.GetBotId()!);


    protected abstract Task InnerProcess(Message message, CancellationToken token);
}