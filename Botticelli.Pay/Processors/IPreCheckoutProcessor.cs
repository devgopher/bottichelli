using Botticelli.Pay.Handlers;
using Botticelli.Pay.Models;

namespace Botticelli.Pay.Processors;

/// <summary>
///     PreCheckout entity processor interface
/// </summary>
// ReSharper disable once UnusedTypeParameter
public interface IPreCheckoutProcessor<THandler>
    where THandler : IPreCheckoutHandler
{
    public Task<(bool isSuccess, string errorMessage)> Process(PreCheckoutQuery request,
        CancellationToken token);
}