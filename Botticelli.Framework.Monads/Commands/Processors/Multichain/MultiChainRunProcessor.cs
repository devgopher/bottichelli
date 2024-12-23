using Botticelli.Client.Analytics;
using Botticelli.Framework.Commands.Processors;
using Botticelli.Framework.Commands.Utils;
using Botticelli.Framework.Commands.Validators;
using Botticelli.Framework.Monads.Commands.Context;
using Botticelli.Shared.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Botticelli.Framework.Monads.Commands.Processors.Multichain;

public class MultiChainRunProcessor<TCommand>(
    ILogger<MultiChainRunProcessor<TCommand>> logger,
    ICommandValidator<TCommand> validator,
    MetricsProcessor metricsProcessor,
    MultiChainRunner<TCommand> multiChainRunner,
    IChoiseResolver resolver,
    IValidator<Message> messageValidator)
    : CommandProcessor<TCommand>(logger, validator, metricsProcessor, messageValidator)
    where TCommand : class, IChainCommand, new()
{
    protected override async Task InnerProcess(Message message, CancellationToken token)
    {
        var command = new TCommand
        {
            Context = new CommandContext()
        };

        command.Context.Set(Names.Message, message);
        command.Context.Set(Names.Args, message.Body.GetArguments());

        _ = await multiChainRunner.Run(command);
    }
}