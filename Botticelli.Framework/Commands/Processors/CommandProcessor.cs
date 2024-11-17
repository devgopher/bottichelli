using System.Text.RegularExpressions;
using Botticelli.Analytics.Shared.Metrics;
using Botticelli.Bot.Interfaces.Processors;
using Botticelli.Bot.Utils;
using Botticelli.Client.Analytics;
using Botticelli.Framework.Commands.Validators;
using Botticelli.Interfaces;
using Botticelli.Shared.API.Client.Requests;
using Botticelli.Shared.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Botticelli.Framework.Commands.Processors;

public abstract partial class CommandProcessor<TCommand> : ICommandProcessor
    where TCommand : class, ICommand
{
    private readonly string _command;
    protected readonly ILogger Logger;
    private readonly MetricsProcessor _metricsProcessor;
    private readonly ICommandValidator<TCommand> _commandValidator;
    private readonly IValidator<Message> _messageValidator;
    protected IBot Bot;

    protected CommandProcessor(ILogger logger,
                               ICommandValidator<TCommand> commandValidator,
                               MetricsProcessor metricsProcessor, 
                               IValidator<Message> messageValidator)
    {
        Logger = logger;
        _commandValidator = commandValidator;
        _metricsProcessor = metricsProcessor;
        _messageValidator = messageValidator;
        _command = GetOldFashionedCommandName(typeof(TCommand).Name);
    }

    protected void Classify(ref Message message)
    {
        var body = GetBody(message);
        
        if (SimpleCommandRegex().IsMatch(body))
            message.Type = Message.MessageType.Command;
        else if (ArgsCommandRegex().IsMatch(body))
            message.Type = Message.MessageType.Command;
        else message.Type = Message.MessageType.Messaging;
    }

    private static string GetBody(Message message) =>
            !string.IsNullOrWhiteSpace(message.CallbackData) ? message.CallbackData : !string.IsNullOrWhiteSpace(message.Body) 
                    ? message.Body : string.Empty;

    public virtual async Task ProcessAsync(Message message, CancellationToken token)
    {
        try
        {
            var messageValidationResult = await _messageValidator.ValidateAsync(message, token);
            if (!messageValidationResult.IsValid)
            {
                _metricsProcessor.Process(MetricNames.BotError, BotDataUtils.GetBotId());
                Logger.LogError(
                    $"Error in {GetType().Name} invalid input message: {messageValidationResult.Errors.Select(e => $"({e.PropertyName} : {e.ErrorCode} : {e.ErrorMessage})")}");
                
                return;
            }
            
            if (message.From!.Id!.Equals(Bot.BotUserId, StringComparison.InvariantCulture)) return;

            Classify(ref message);
            
            if (string.IsNullOrWhiteSpace(message.Body) &&
                message.Attachments == default &&
                message.Location == default &&
                message.Contact == default &&
                message.Poll == default &&
                message.CallbackData == default)
            {
                Logger.LogWarning("Message {msgId} is empty! Skipping...", message.Uid);

                return;
            }

            // if we've any callback data, lets assume , that it is a command, if not - see in a message body
            var body = GetBody(message);

            if (SimpleCommandRegex().IsMatch(body))
            {
                var match = SimpleCommandRegex().Matches(body)
                    .FirstOrDefault();

                if (match == default) return;

                var commandName = GetOldFashionedCommandName(match.Groups[1].Value);

                if (commandName != _command) return;

                await ValidateAndProcess(message,
                    string.Empty,
                    token);

                SendMetric(MetricNames.CommandReceived);
            }
            else if (ArgsCommandRegex().IsMatch(body))
            {
                var match = ArgsCommandRegex().Matches(body)
                    .FirstOrDefault();

                if (match == default) return;

                var argsString = match.Groups[2].Value;

                var commandName = GetOldFashionedCommandName(match.Groups[1].Value);

                if (commandName != _command) return;

                await ValidateAndProcess(message,
                    argsString,
                    token);

                SendMetric(MetricNames.CommandReceived);
            }
            else
            {
                if (GetType().IsAssignableTo(typeof(CommandChainProcessor<TCommand>)))
                    await ValidateAndProcess(message,
                                             string.Empty,
                                             token);
            }

            if (message.Location != default) await InnerProcessLocation(message, string.Empty, token);
            if (message.Poll != default) await InnerProcessPoll(message, string.Empty, token);
            if (message.Contact != default) await InnerProcessContact(message, string.Empty, token);
        }
        catch (Exception ex)
        {
            _metricsProcessor.Process(MetricNames.BotError, BotDataUtils.GetBotId());
            Logger.LogError(ex, $"Error in {GetType().Name}: {ex.Message}");
        }
    }

    public virtual void SetBot(IBot bot)
        => Bot = bot;

    public void SetServiceProvider(IServiceProvider sp)
    {
    }

    private void SendMetric(string metricName) => _metricsProcessor.Process(metricName, BotDataUtils.GetBotId()!);

    private void SendMetric() => _metricsProcessor.Process(GetOldFashionedCommandName(
        $"{GetType().Name.Replace("Processor", string.Empty)}Command"), BotDataUtils.GetBotId()!);
    
    private string GetOldFashionedCommandName(string fullCommand)
        => fullCommand.ToLowerInvariant().Replace("command", "");

    private async Task ValidateAndProcess(Message message,
        string args,
        CancellationToken token)
    {
        if (message.Type == Message.MessageType.Messaging)
        {
            SendMetric();

            await InnerProcess(message, args, token);

            return;
        }
        
        if (await _commandValidator.Validate(message.Body))
        {
            SendMetric();
            await InnerProcess(message, args, token);
        }
        else
        {
            var errMessageRequest = new SendMessageRequest
            {
                Message =
                {
                    Body = _commandValidator.Help()
                }
            };

            await Bot.SendMessageAsync(errMessageRequest, token);
        }
    }

    protected virtual Task InnerProcessContact(Message message, string args, CancellationToken token) => Task.CompletedTask;
    protected virtual Task InnerProcessPoll(Message message, string args, CancellationToken token) => Task.CompletedTask;
    protected virtual Task InnerProcessLocation(Message message, string args, CancellationToken token) => Task.CompletedTask;
    protected abstract Task InnerProcess(Message message, string args, CancellationToken token);
   
    [GeneratedRegex("\\/([a-zA-Z0-9]*)$")]
    private static partial Regex SimpleCommandRegex();
    [GeneratedRegex("\\/([a-zA-Z0-9]*) (.*)")]
    private static partial Regex ArgsCommandRegex();
}