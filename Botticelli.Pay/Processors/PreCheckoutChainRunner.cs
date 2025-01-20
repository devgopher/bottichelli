using Botticelli.Pay.Handlers;
using Botticelli.Pay.Models;

namespace Botticelli.Pay.Processors;

/// <summary>
///     Chain runner
/// </summary>
/// <typeparam name="THandler"></typeparam>
public class PreCheckoutChainRunner<THandler> where THandler : IPreCheckoutHandler
{
    private readonly List<IPreCheckoutProcessor<THandler>> _preCheckoutProcessors;

    public PreCheckoutChainRunner(IEnumerable<IPreCheckoutProcessor<THandler>> preCheckoutProcessors) => _preCheckoutProcessors = preCheckoutProcessors.ToList();

    public PreCheckoutChainRunner() => _preCheckoutProcessors = [];
    
    public async Task<(bool isSuccessful, string errorMessage)> Run(PreCheckoutQuery request,
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