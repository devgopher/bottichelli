using Botticelli.Pay.Handlers;

namespace Botticelli.Pay.Processors;

public class DummyPayProcessor<THandler, TQuery> : IPayProcessor<THandler, TQuery> where THandler : IPayHandler
{
    public Task<(bool isSuccess, string errorMessage)> Process(TQuery request, CancellationToken token) 
        => Task.FromResult((true, string.Empty));
}