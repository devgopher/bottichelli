using Botticelli.Framework.Monads.Commands.Context;
using Botticelli.Interfaces;

namespace Botticelli.Framework.Monads.Commands.Processors.Multichain;

public abstract class MultiChainProcessor<TInChoise, TOutChoice>(IChoiseResolver choiceResolver)
    : IMultiChainProcessor<TInChoise, TOutChoice>
    where TOutChoice : IChoise, new()
    where TInChoise : IChoise
{
    public IBot? Bot { get; set; }

    public void SetBot(IBot bot) => Bot = bot;

    public virtual async Task<TOutChoice> Process(TInChoise choice, CancellationToken token = default)
    {
        if (!choiceResolver.Resolve(choice))
            return new TOutChoice();

        return await InnerProcess(choice, token).ConfigureAwait(false);
    }

    protected abstract Task<TOutChoice> InnerProcess(IChoise choice, CancellationToken token = default);
}