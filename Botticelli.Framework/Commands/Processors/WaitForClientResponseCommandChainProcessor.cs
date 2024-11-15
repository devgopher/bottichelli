using Botticelli.Client.Analytics;
using Botticelli.Framework.Commands.Validators;
using Botticelli.Interfaces;
using Botticelli.Shared.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Botticelli.Framework.Commands.Processors;

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
            
        Classify(ref message);
        message.ChainId = Guid.NewGuid();
        ChainIds.Add(message.ChainId.Value);

        if (message.Type != Message.MessageType.Messaging)
        {
            await base.ProcessAsync(message, token);
            
            return;
        }

        if (DateTime.UtcNow - message.LastModifiedAt > Timeout)
            return;

        message.ProcessingArgs ??= new List<string>();
        message.ProcessingArgs.Add(message.Body);

        if (Next != null)
        {
            Next.ChainIds.Add(message.ChainId.Value);
            await Next.ProcessAsync(message, token);
        }
        else
            Logger.LogInformation("No Next command for message {uid}", message.Uid);
    }

    public HashSet<Guid> ChainIds { get; } = new(1000);
    public ICommandChainProcessor Next { get; set; }
}