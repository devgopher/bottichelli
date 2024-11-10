using Botticelli.Framework.Commands;
using Botticelli.Framework.Monads.Commands.Context;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Botticelli.Framework.Monads.Commands.Processors;

public class ChainBuilder<TCommand>(IServiceCollection services)
    where TCommand : IChainCommand
{
    private readonly List<IChainProcessor<TCommand>> _chain = new(5);
    private ChainRunner<TCommand>? _runner;

    public ChainBuilder<TCommand> AddElement(IChainProcessor<TCommand> processor)
    {
        _chain.Add(processor);
        
        return this;
    }

    public ChainBuilder<TCommand> AddElement<TProcessor>() 
        where TProcessor : IChainProcessor<TCommand>
    {
        var processor = services.BuildServiceProvider()
            .GetRequiredService<TProcessor>();
        
        return AddElement(processor);
    }
    
    public ChainRunner<TCommand> Build()
    {
        _runner ??= new ChainRunner<TCommand>(_chain, services.BuildServiceProvider()
            .GetRequiredService<ILogger<ChainRunner<TCommand>>>());
        
        return _runner;
    }
}