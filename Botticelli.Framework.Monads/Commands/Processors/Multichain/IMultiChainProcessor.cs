using Botticelli.Interfaces;

namespace Botticelli.Framework.Monads.Commands.Processors.Multichain;

/// <summary>
///     Chain processor with multipath processing
/// </summary>
/// <typeparam name="TOutChoice">Output choice type</typeparam>
/// <typeparam name="TInChoise">Input choise type</typeparam>
public interface IMultiChainProcessor<TInChoise, TOutChoice>
where TOutChoice : IChoise
where TInChoise : IChoise
{
    public IBot? Bot { get; }

    public void SetBot(IBot bot);

    public Task<TOutChoice> Process(TInChoise choice, CancellationToken token = default);
}