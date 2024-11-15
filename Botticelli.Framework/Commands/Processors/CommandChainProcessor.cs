using System.Collections.Concurrent;
using Botticelli.Bot.Interfaces.Processors;
using Botticelli.Client.Analytics;
using Botticelli.Framework.Commands.Validators;
using Botticelli.Interfaces;
using Botticelli.Shared.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Botticelli.Framework.Commands.Processors;

/// <summary>
/// A command chain processor means, that we've a command on input and several step to process it
/// During a processing every processor changes values in 
/// </summary>
/// <typeparam name="TInputCommand"></typeparam>
public abstract class CommandChainProcessor<TInputCommand> : CommandProcessor<TInputCommand>, ICommandChainProcessor<TInputCommand>
        where TInputCommand : class, ICommand
{
    public CommandChainProcessor(ILogger<CommandChainProcessor<TInputCommand>> logger,
                                 ICommandValidator<TInputCommand> commandValidator,
                                 MetricsProcessor metricsProcessor,  IValidator<Message> messageValidator) 
        : base(logger, commandValidator, metricsProcessor, messageValidator)
    {
    }

    public HashSet<Guid> ChainIds { get; } = new(100);
    public ICommandChainProcessor Next { get; set; }
}