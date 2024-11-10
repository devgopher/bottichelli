using Botticelli.Framework.Monads.Commands.Context;
using Botticelli.Framework.Monads.Commands.Processors;
using Microsoft.Extensions.DependencyInjection;

namespace Botticelli.Framework.Monads.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMonadsChain<TCommand>(this IServiceCollection services,
        Func<ChainBuilder<TCommand>> func) where TCommand : class, IChainCommand, new()
    {
        var chainBuilder = func();
        var runner = chainBuilder.Build(services.BuildServiceProvider());

        services.AddScoped<ChainRunProcessor<TCommand>>()
            .AddScoped(_ => runner);

        return services;
    }
}