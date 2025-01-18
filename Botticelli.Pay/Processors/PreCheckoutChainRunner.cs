using Botticelli.Pay.Handlers;
using Botticelli.Pay.Models;

namespace Botticelli.Pay.Processors;

/// <summary>
///     Chain runner
/// </summary>
/// <param name="preCheckoutProcessors"></param>
/// <typeparam name="THandler"></typeparam>
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