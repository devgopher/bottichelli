using Botticelli.Client.Analytics;
using Botticelli.Framework.Commands;
using Botticelli.Framework.Commands.Processors;
using Botticelli.Framework.Commands.Validators;
using Botticelli.Framework.Monads.Commands.Context;
using Botticelli.Shared.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Botticelli.Framework.Monads.Commands.Processors;

public class ChainRunProcessor<TCommand>(
    ILogger<ChainRunProcessor<TCommand>> logger,
    ICommandValidator<TCommand> validator,
    MetricsProcessor metricsProcessor,
    ChainRunner<TCommand> chainRunner)
    : CommandProcessor<TCommand>(logger, validator, metricsProcessor)
    where TCommand : class, IChainCommand, new()
{
    protected override async Task InnerProcess(Message message, string args, CancellationToken token)
    {
        var command = new TCommand()
        {
            Context = new CommandContext()
        };
        
        command.Context.Set("message", message);
        command.Context.Set("args", args);
        
        _ = await chainRunner.Run(command);
    }
}