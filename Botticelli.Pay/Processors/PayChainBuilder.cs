using Botticelli.Pay.Handlers;
using Botticelli.Pay.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Botticelli.Pay.Processors;

/// <summary>
///     Chain builder
/// </summary>
/// <typeparam name="THandler"></typeparam>
/// <typeparam name="TProcessor"></typeparam>
/// <typeparam name="TQuery"></typeparam>
public class PayChainBuilder<THandler, TProcessor, TQuery>
where THandler : IPayHandler
where TProcessor : IPayProcessor<THandler, TQuery>
{
    private readonly List<IPayProcessor<THandler, TQuery>> _preCheckoutProcessors = new(10);

    public void AddElement<T>(Func<T, T> func)
        where T : TProcessor, new()
    {
        var element = new T();
        element = func(element);

        AddElement(element);
    }

    public PayChainBuilder<THandler, TProcessor, TQuery> AddElement(TProcessor element)
    {
        _preCheckoutProcessors.Add(element);
        
        return this;
    }

    public PayChainRunner<THandler, TQuery> Build() => new(_preCheckoutProcessors);
}