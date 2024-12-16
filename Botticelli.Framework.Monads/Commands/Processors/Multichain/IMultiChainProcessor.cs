using Botticelli.Framework.Monads.Commands.Context;
using Botticelli.Framework.Monads.Commands.Result;
using Botticelli.Interfaces;
using LanguageExt;

namespace Botticelli.Framework.Monads.Commands.Processors.Multichain;

/// <summary>
///     Chain processor with multipath processing
/// </summary>
/// <typeparam name="TCommand">Command</typeparam>
/// <typeparam name="TNeededType">Needed type for a particular processor</typeparam>
/// <typeparam name="TOutChoice">Output choice type</typeparam>
public interface IMultiChainProcessor<TCommand, TNeededType, TOutChoice> where TCommand : IChainCommand
where TOutChoice : IChoise
{
    public IBot? Bot { get; }

    public void SetBot(IBot bot);

    public Task<TOutChoice> Process(IChoise choice, CancellationToken token = default);
}


public abstract class MultiChainProcessor<TCommand, TNeededType, TOutChoice>(IChoiseResolver<TNeededType> choiceResolver)
    : IMultiChainProcessor<TCommand, TNeededType, TOutChoice>
    where TCommand : IChainCommand 
    where TOutChoice : IChoise, new()
{
    public IBot? Bot { get; set; }

    public void SetBot(IBot bot) => Bot = bot;

    public virtual async Task<TOutChoice> Process(IChoise choice, CancellationToken token = default)
    {
        if (!choiceResolver.Resolve(choice))
            return new TOutChoice();

        return await InnerProcess(choice, token).ConfigureAwait(false);
    }

    protected abstract Task<TOutChoice> InnerProcess(IChoise choice, CancellationToken token = default);
}