using Botticelli.Framework.Monads.Commands.Context;
using Botticelli.Framework.Monads.Commands.Result;
using Botticelli.Interfaces;
using Botticelli.Shared.ValueObjects;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace Botticelli.Framework.Monads.Commands.Processors;

/// <summary>
///     Chain processor
/// </summary>
/// <typeparam name="TCommand"></typeparam>
public abstract class ChainProcessor<TCommand>(ICommandContext commandContext, ILogger logger)
    : IChainProcessor<TCommand>
    where TCommand : IChainCommand
{
    protected readonly ILogger Logger = logger;

    public IBot Bot { get; private set; }

    public void SetBot(IBot bot) => Bot = bot;
    
    public virtual async Task<Either<FailResult<TCommand>, SuccessResult<TCommand>>> Process(
        Either<FailResult<TCommand>, SuccessResult<TCommand>> stepResult, CancellationToken token = default)
    {
        try
        {
            if (stepResult.IsRight) await InnerProcessAsync((IResult<TCommand>)stepResult.Case, token);

            return stepResult;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
            var errResult = (IResult<TCommand>)stepResult.Case;
            await InnerErrorProcessAsync((IResult<TCommand>)stepResult.Case, token);

            return FailResult<TCommand>.Create(errResult.Command,
                ex.Message,
                GetMessage(errResult.Command));
        }
    }

    protected abstract Task InnerProcessAsync(IResult<TCommand> stepResult, CancellationToken token);

    protected abstract Task InnerErrorProcessAsync(IResult<TCommand> command, CancellationToken token);

    protected Message? GetMessage(TCommand command) => command.Context.Get<Message>("message");
    
    protected string? GetArgs(TCommand command) => command.Context.Get<string>("args");
}