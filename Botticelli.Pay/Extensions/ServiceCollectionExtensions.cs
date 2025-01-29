using Botticelli.Pay.Handlers;
using Botticelli.Pay.Models;
using Botticelli.Pay.Processors;
using Microsoft.Extensions.DependencyInjection;

namespace Botticelli.Pay.Extensions;

public static class ServiceCollectionExtensions
{
    public static PayChainBuilder<THandler, TProcessor, TQuery> AddPayPreCheckouts<THandler, TProcessor, TQuery>(
        this IServiceCollection services,
        params TProcessor[] preCheckoutProcessors)
        where THandler : IPayHandler
        where TProcessor : IPayProcessor<THandler, TQuery>, new()
    {
        var builder = AddPayPreCheckout<THandler, TProcessor, TQuery>(services);

        foreach (var processor in preCheckoutProcessors)
            builder.AddElement(processor);

        return builder;
    }

    public static PayChainBuilder<THandler, TProcessor, TQuery> AddPayPreCheckout<THandler, TProcessor, TQuery>(
        this IServiceCollection services,
        TProcessor processor)
        where THandler : IPreCheckoutHandler, new()
        where TProcessor : IPayProcessor<THandler, TQuery>, new()
        => AddPayPreCheckouts<THandler, TProcessor, TQuery>(services)
            .AddElement(processor);

    public static PayChainBuilder<THandler, TProcessor, TQuery> AddPayPreCheckout<THandler, TProcessor, TQuery>(
        this IServiceCollection services)
        where THandler : IPayHandler where TProcessor : IPayProcessor<THandler, TQuery>
    {
        services.AddSingleton<PayChainBuilder<THandler, TProcessor, TQuery>>();

        return services.BuildServiceProvider().GetRequiredService<PayChainBuilder<THandler, TProcessor, TQuery>>();
    }

    public static PayChainBuilder<THandler, TProcessor, TQuery>
        AddPaySuccessful<THandler, TProcessor, TQuery>(this IServiceCollection services) where THandler : IPayHandler
        where TProcessor : IPayProcessor<THandler, TQuery>
    {
        services.AddSingleton<PayChainBuilder<THandler, TProcessor, TQuery>>();

        return services.BuildServiceProvider().GetRequiredService<PayChainBuilder<THandler, TProcessor, TQuery>>();
    }

    public static PayChainBuilder<THandler, TProcessor, TQuery>
        AddPayError<THandler, TProcessor, TQuery>(this IServiceCollection services) where THandler : IPayHandler
        where TProcessor : IPayProcessor<THandler, TQuery>
    {
        services.AddSingleton<PayChainBuilder<THandler, TProcessor, TQuery>>();

        return services.BuildServiceProvider().GetRequiredService<PayChainBuilder<THandler, TProcessor, TQuery>>();
    }

    public static PayChainRunner<THandler, TQuery> AddPayments<THandler, TProcessor, TQuery>(
        this IServiceCollection services)
        where THandler : IPayHandler where TProcessor : IPayProcessor<THandler, TQuery>
    {
        var builder = AddPayPreCheckout<THandler, TProcessor, TQuery>(services);
        var runner = builder.Build();

        services.AddScoped<PayChainRunner<THandler, TQuery>>(_ => runner);

        return runner;
    }
}