using Botticelli.Pay.Handlers;
using Botticelli.Pay.Processors;
using Microsoft.Extensions.DependencyInjection;

namespace Botticelli.Pay.Extensions;

public static class ServiceCollectionExtensions
{
    public static PreCheckoutChainBuilder<THandler> AddPayPreCheckouts<THandler>(
        this IServiceCollection services,
        params IPreCheckoutProcessor<THandler>[] preCheckoutProcessors)
        where THandler : IPreCheckoutHandler, new()
    {
        var builder = AddPayPreCheckout<THandler>(services);

        foreach (var processor in preCheckoutProcessors)
            builder.AddElement(processor);

        return builder;
    }

    public static PreCheckoutChainBuilder<THandler> AddPayPreCheckout<THandler>(
        this IServiceCollection services,
        IPreCheckoutProcessor<THandler> processor)
        where THandler : IPreCheckoutHandler, new() =>
        AddPayPreCheckouts<THandler>(services)
            .AddElement(processor);

    public static PreCheckoutChainBuilder<THandler> AddPayPreCheckout<THandler>(this IServiceCollection services)
        where THandler : IPreCheckoutHandler
    {
        services.AddSingleton<PreCheckoutChainBuilder<THandler>>();

        return services.BuildServiceProvider().GetRequiredService<PreCheckoutChainBuilder<THandler>>();
    }

    public static PreCheckoutChainRunner<THandler> AddPayments<THandler>(this IServiceCollection services)
        where THandler : IPreCheckoutHandler
    {
        var builder = AddPayPreCheckout<THandler>(services);
        var runner = builder.Build();

        services.AddScoped<PreCheckoutChainRunner<THandler>>(_ => runner);
        
        return runner;
    }
}