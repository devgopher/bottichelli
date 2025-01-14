using Botticelli.Pay.Handlers;
using Botticelli.Pay.Models;

namespace Botticelli.Pay.Processors;

/// <summary>
/// PreCheckout entity processor interface
/// </summary>
public interface IPreCheckoutProcessor<THandler>
where THandler : IPreCheckoutHandler
{
    public Task<(bool isSuccess, string errorMessage)> Process(PreCheckoutQuery request,
        CancellationToken token);
}

public class PreCheckoutChainBuilder<THandler>
    where THandler : IPreCheckoutHandler
{
    private readonly List<IPreCheckoutProcessor<THandler>> _preCheckoutProcessors = new(10);

    public void AddElement<T>(Action<T> func)
    where T : IPreCheckoutProcessor<THandler>
    {
        
        func();
    }

    public PreCheckoutChainRunner<THandler> Build() => new(_preCheckoutProcessors);
}

public class PreCheckoutChainRunner<THandler>(IEnumerable<IPreCheckoutProcessor<THandler>> preCheckoutProcessors)
    where THandler : IPreCheckoutHandler
{
    private readonly List<IPreCheckoutProcessor<THandler>> _preCheckoutProcessors = preCheckoutProcessors.ToList();

    public async Task<(bool isSuccessful, string errorMessage)> Run(PreCheckoutQuery request,
        CancellationToken token)
    {
        (bool isSuccessful, string errorMessage) procResult = default;

        foreach (var processor in _preCheckoutProcessors)
        {
            procResult = await processor.Process(request, token);
            if (!procResult.isSuccessful)
                break;
        }
        
        return procResult;
    }
}