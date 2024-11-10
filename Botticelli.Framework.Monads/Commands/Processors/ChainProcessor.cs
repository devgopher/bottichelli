using Botticelli.Framework.Monads.Commands.Context;
using Botticelli.Framework.Monads.Commands.Result;
using Botticelli.Interfaces;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace Botticelli.Framework.Monads.Commands.Processors;

/// <summary>
///     Chain processor
/// </summary>
/// <typeparam name="TCommand"></typeparam>
public abstract class ChainProcessor<TCommand>(ICommandContext commandContext, ILogger<ChainProcessor<TCommand>> logger)
    : IChainProcessor<TCommand>
    where TCommand : IChainCommand
{
    protected readonly ILogger<ChainProcessor<TCommand>> Logger = logger;

    public IBot Bot { get; set; }

    public async Task<Either<FailResult<TCommand>, SuccessResult<TCommand>>> Process(
        Either<FailResult<TCommand>, SuccessResult<TCommand>> command, CancellationToken token = default)
    {
        try
        {
            if (command.IsRight) await InnerProcessAsync((IResult<TCommand>)command.Case, token);

            return command;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
            var errCmd = (IResult<TCommand>)command.Case;
            await InnerErrorProcessAsync((IResult<TCommand>)command.Case, token);

            return FailResult<TCommand>.Create(errCmd.Command,
                commandContext,
                ex.Message,
                errCmd.Message);
        }
    }

    protected abstract Task InnerProcessAsync(IResult<TCommand> command, CancellationToken token);

    protected abstract Task InnerErrorProcessAsync(IResult<TCommand> command, CancellationToken token);
}