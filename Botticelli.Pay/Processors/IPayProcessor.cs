using Botticelli.Pay.Handlers;
using Botticelli.Pay.Models;

namespace Botticelli.Pay.Processors;

/// <summary>
///     PreCheckout entity processor interface
/// </summary>
// ReSharper disable once UnusedTypeParameter
public interface IPayProcessor<THandler, in TQuery>
    where THandler : IPayHandler
{
    public Task<(bool isSuccess, string errorMessage)> Process(TQuery request,
        CancellationToken token);
}