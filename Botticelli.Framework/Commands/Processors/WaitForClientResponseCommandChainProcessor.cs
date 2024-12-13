using System.Collections.Concurrent;
using Botticelli.Client.Analytics;
using Botticelli.Framework.Commands.Validators;
using Botticelli.Interfaces;
using Botticelli.Shared.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Botticelli.Framework.Commands.Processors;

public static class ChainStateKeeper
{
    private static readonly ConcurrentDictionary<string, bool> IsChainOpened = new();

    public static bool GetState(string chatId) => IsChainOpened.TryGetValue(chatId, out var isChainOpened) && isChainOpened;
    
    public static void SetState(string chatId, bool isChainOpened) => IsChainOpened[chatId] = isChainOpened;

    public static void SetState(IEnumerable<string> chatIds, bool isChainOpened)
    {
        foreach (var chatId in chatIds) SetState(chatId, isChainOpened);
    }
}

/// <summary>
///     A command with waiting for a response
/// </summary>
/// <typeparam name="TInputCommand"></typeparam>
public abstract class WaitForClientResponseCommandChainProcessor<TInputCommand> : CommandProcessor<TInputCommand>,
    ICommandChainProcessor<TInputCommand>
    where TInputCommand : class, ICommand
{
    protected WaitForClientResponseCommandChainProcessor(ILogger<CommandChainProcessor<TInputCommand>> logger,
        ICommandValidator<TInputCommand> commandValidator,
        MetricsProcessor metricsProcessor,
        IValidator<Message> messageValidator)
        : base(logger, commandValidator, metricsProcessor, messageValidator)
    {
    }

    private TimeSpan Timeout { get; } = TimeSpan.FromMinutes(10);

    public virtual void SetBot(IBot bot) => Bot = bot;

    public override async Task ProcessAsync(Message message, CancellationToken token)
    {
        // filters 'not our' chains
        if (message.ChainId != null && !ChainIds.Contains(message.ChainId.Value))
            return;

        message.ChainId ??= Guid.NewGuid();
        Classify(ref message);
        ChainIds.Add(message.ChainId.Value);

        if (message.Type != Message.MessageType.Messaging)
        {
            // sets input state to true
            ChainStateKeeper.SetState(message.ChatIds.Single(), true);
            await base.ProcessAsync(message, token);

            return;
        }

        if (DateTime.UtcNow - message.LastModifiedAt > Timeout)
            return;

        var chatId = message.ChatIds.Single();
        
        // checks if input state = true
        if (!ChainStateKeeper.GetState(chatId))
            return;
        
        message.ProcessingArgs ??= new List<string>();
        message.ProcessingArgs.Add(message.Body!);

        if (Next != null)
        {
            Next.ChainIds.Add(message.ChainId.Value);
            await Next.ProcessAsync(message, token);
        }
        else
        {
            Logger.LogInformation("No Next command for message {uid}", message.Uid);
        }
    }

    public HashSet<Guid> ChainIds { get; } = new(1000);
    public ICommandChainProcessor Next { get; set; }
}