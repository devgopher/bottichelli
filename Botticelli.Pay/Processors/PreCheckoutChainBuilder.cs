using Botticelli.Pay.Handlers;

namespace Botticelli.Pay.Processors;

/// <summary>
///     PreCheckout chain builder
/// </summary>
/// <typeparam name="THandler"></typeparam>
public class PreCheckoutChainBuilder<THandler>
    where THandler : IPreCheckoutHandler
{
    private readonly List<IPreCheckoutProcessor<THandler>> _preCheckoutProcessors = new(10);

    public void AddElement<T>(Func<T, T> func)
        where T : IPreCheckoutProcessor<THandler>, new()
    {
        var element = new T();
        element = func(element);

        AddElement(element);
    }

    public PreCheckoutChainBuilder<THandler> AddElement<T>(T element)
        where T : IPreCheckoutProcessor<THandler>
    {
        _preCheckoutProcessors.Add(element);
        
        return this;
    }

    public PreCheckoutChainRunner<THandler> Build() => new(_preCheckoutProcessors);
}