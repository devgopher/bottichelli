using Botticelli.Client.Analytics;
using Botticelli.Framework.Commands.Validators;
using Botticelli.Shared.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Botticelli.Framework.Commands.Processors;

public class CommandChainFirstElementProcessor<TInputCommand> : CommandChainProcessor<TInputCommand>, ICommandChainFirstElementProcessor
        where TInputCommand : class, ICommand
{
    public CommandChainFirstElementProcessor(ILogger<CommandChainProcessor<TInputCommand>> logger,
                                             ICommandValidator<TInputCommand> commandValidator,
                                             MetricsProcessor metricsProcessor,
                                             IValidator<Message> messageValidator)
            : base(logger, commandValidator, metricsProcessor, messageValidator)
    {
    }

    public ICommandChainProcessor Next { get; set; }

    public override async Task ProcessAsync(Message message, CancellationToken token)
    {
        Logger.LogDebug(Next == default ?
                                 $"{nameof(CommandChainProcessor<TInputCommand>)} : no next step, returning" :
                                 $"{nameof(CommandChainProcessor<TInputCommand>)} : next step is '{Next?.GetType().Name}'");

        if (Next != default) await Next.ProcessAsync(message, token)!;
    }

    protected override Task InnerProcess(Message message, CancellationToken token)
        => Task.CompletedTask;
}