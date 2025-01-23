using Botticelli.Pay.Handlers;
using Botticelli.Pay.Models;

namespace Botticelli.Pay.Processors;

/// <summary>
///     Chain runner
/// </summary>
/// <typeparam name="THandler"></typeparam>
/// <typeparam name="TQuery"></typeparam>
public class PayChainRunner<THandler, TQuery> where THandler : IPayHandler
{
    private readonly List<IPayProcessor<THandler, TQuery>> _preCheckoutProcessors;

    public PayChainRunner(IEnumerable<IPayProcessor<THandler, TQuery>> preCheckoutProcessors) 
        => _preCheckoutProcessors = preCheckoutProcessors.ToList();

    public PayChainRunner() => _preCheckoutProcessors = [];
    
    public async Task<(bool isSuccessful, string errorMessage)> Run(TQuery request,
        CancellationToken token)
    {
        (bool isSuccessful, string errorMessage) procResult = (true, string.Empty);

        foreach (var processor in _preCheckoutProcessors)
        {
            procResult = await processor.Process(request, token);
            if (!procResult.isSuccessful)
                break;
        }

        return procResult;
    }
}