using Botticelli.Framework.Monads.Commands.Context;
using Botticelli.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Botticelli.Framework.Monads.Commands.Processors.Multichain;

public class MultiChainBuilder<TCommand>(IServiceCollection services)
    where TCommand : IChainCommand
{
    private readonly List<IMultiChainProcessor<IChoise>> _chain = new(5);
    private IBot? _bot;
    private MultiChainRunner<TCommand>? _runner;

    public MultiChainBuilder<TCommand> Next(IMultiChainProcessor<IChoise> processor)
    {
        _chain.Add(processor);

        return this;
    }

    public MultiChainBuilder<TCommand> Next<TProcessor>()
        where TProcessor : class, IMultiChainProcessor<IChoise>
    {
        services.AddScoped<TProcessor>();
        var processor = services.BuildServiceProvider()
            .GetRequiredService<TProcessor>();

        return Next(processor);
    }

    public MultiChainBuilder<TCommand> Next<TProcessor>(Action<TProcessor> func)
        where TProcessor : class, IMultiChainProcessor<IChoise>
    {
        services.AddScoped<TProcessor>();
        var processor = services.BuildServiceProvider()
            .GetRequiredService<TProcessor>();
        func(processor);

        return Next(processor);
    }

    public MultiChainBuilder<TCommand> SetBot<TBot>(TBot bot)
        where TBot : IBot<TBot>
    {
        _bot = bot;

        return this;
    }

    public MultiChainRunner<TCommand> Build()
    {
        var sp = services.BuildServiceProvider();
        _bot ??= sp.GetServices<IBot>().FirstOrDefault();

        if (_bot == default)
            throw new NullReferenceException($"Bot should be set up: call {nameof(SetBot)} to set a bot instance!");

        foreach (var processor in _chain) processor.SetBot(_bot);

        _runner ??= new MultiChainRunner<TCommand>(_chain, sp
            .GetRequiredService<ILogger<MultiChainRunner<TCommand>>>());

        return _runner;
    }
}