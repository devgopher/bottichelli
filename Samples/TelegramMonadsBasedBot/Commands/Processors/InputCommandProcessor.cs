using Botticelli.Framework.Monads.Commands.Context;
using Botticelli.Framework.Monads.Commands.Processors;
using Botticelli.Framework.Monads.Commands.Result;

namespace TelegramMonadsBasedBot.Commands.Processors;

public class InputCommandProcessor<TCommand>(ILogger<InputCommandProcessor<TCommand>> logger)
    : ChainProcessor<TCommand>(new CommandContext(), logger)
    where TCommand : IChainCommand
{
    protected override Task InnerProcessAsync(IResult<TCommand> stepResult, CancellationToken token)
    {
        stepResult.Command.Context.Set("input", args);
        
        return Task.CompletedTask;
    }

    protected override Task InnerErrorProcessAsync(IResult<TCommand> command, CancellationToken token) => throw new NotImplementedException();
}