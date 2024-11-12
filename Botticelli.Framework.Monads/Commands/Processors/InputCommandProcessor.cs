using Botticelli.Framework.Monads.Commands.Context;
using Botticelli.Framework.Monads.Commands.Result;
using Microsoft.Extensions.Logging;

namespace Botticelli.Framework.Monads.Commands.Processors;

public class InputCommandProcessor<TCommand>(ILogger<InputCommandProcessor<TCommand>> logger)
    : ChainProcessor<TCommand>(logger)
    where TCommand : IChainCommand
{
    protected override Task InnerProcessAsync(IResult<TCommand> stepResult, CancellationToken token) => Task.CompletedTask;

    protected override Task InnerErrorProcessAsync(FailResult<TCommand> stepResult, CancellationToken token) => Task.CompletedTask;
}