using Botticelli.Framework.Commands;
using Botticelli.Framework.Monads.Commands.Context;
using Botticelli.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Botticelli.Framework.Monads.Commands.Processors;

public class ChainBuilder<TCommand>(IServiceCollection services)
    where TCommand : IChainCommand
{
    private readonly List<IChainProcessor<TCommand>> _chain = new(5);
    private IBot? _bot;
    private ChainRunner<TCommand>? _runner;

    public ChainBuilder<TCommand> AddElement(IChainProcessor<TCommand> processor)
    {
        _chain.Add(processor);
        
        return this;
    }

    public ChainBuilder<TCommand> AddElement<TProcessor>() 
        where TProcessor : class, IChainProcessor<TCommand>
    {
        services.AddScoped<TProcessor>();
        var processor = services.BuildServiceProvider()
            .GetRequiredService<TProcessor>();
        
        return AddElement(processor);
    }
    
    public ChainBuilder<TCommand> SetBot<TBot>(TBot bot) 
        where TBot : IBot<TBot>
    {
        _bot = bot;
   
        return this;
    }
    
    public ChainRunner<TCommand> Build()
    {
        var sp = services.BuildServiceProvider();
        _bot ??= sp.GetServices<IBot>().FirstOrDefault();

        if (_bot == default)
            throw new NullReferenceException($"Bot should be set up: call {nameof(SetBot)} to set a bot instance!");
            
        foreach (var processor in _chain) processor.SetBot(_bot);
        
        _runner ??= new ChainRunner<TCommand>(_chain, sp
            .GetRequiredService<ILogger<ChainRunner<TCommand>>>());
        
        return _runner;
    }
}