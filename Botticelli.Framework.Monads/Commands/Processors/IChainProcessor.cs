using Botticelli.Framework.Monads.Commands.Context;
using Botticelli.Framework.Monads.Commands.Result;
using Botticelli.Interfaces;
using LanguageExt;

namespace Botticelli.Framework.Monads.Commands.Processors;

/// <summary>
///     Chain processor
/// </summary>
/// <typeparam name="TCommand"></typeparam>
/// <typeparam name="TBot"></typeparam>
public interface IChainProcessor<TCommand> where TCommand : IChainCommand
{
    public IBot? Bot { get; }

    public void SetBot(IBot bot);

    public Task<EitherAsync<FailResult<TCommand>, SuccessResult<TCommand>>> Process(
        EitherAsync<FailResult<TCommand>, SuccessResult<TCommand>> stepResult, CancellationToken token = default);
}