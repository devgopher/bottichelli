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
public abstract class ChainProcessor<TCommand>(ILogger logger)
    : IChainProcessor<TCommand>
    where TCommand : IChainCommand
{
    protected readonly ILogger Logger = logger;

    public IBot Bot { get; private set; }

    public void SetBot(IBot bot) => Bot = bot;

    public virtual async Task<EitherAsync<FailResult<TCommand>, SuccessResult<TCommand>>> Process(
        EitherAsync<FailResult<TCommand>, SuccessResult<TCommand>> stepResult, CancellationToken token = default)
    {
        var unit = await stepResult.BiIter(r => InnerProcessAsync(r, token),
            l => InnerErrorProcessAsync(l, token));

        return stepResult;
    }

    protected abstract Task InnerProcessAsync(IResult<TCommand> stepResult, CancellationToken token);

    protected virtual Task InnerErrorProcessAsync(FailResult<TCommand> stepResult, CancellationToken token)
    {
        var message = GetMessage(stepResult.Command);
        Logger.LogError(message?.Body ?? $"Error in: {nameof(ChainProcessor<TCommand>)}");

        return Task.CompletedTask;
    }

    protected Message? GetMessage(TCommand command) => command.Context.Get<Message>("message");

    protected string? GetArgs(TCommand command) => command.Context.Get("args");
}