using Botticelli.Framework.Commands;
using Botticelli.Framework.Monads.Commands.Context;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Botticelli.Framework.Monads.Commands.Processors;

public class ChainBuilder<TCommand> where TCommand : IChainCommand
{
    private readonly List<IChainProcessor<TCommand>> _chain = new(5);
    private ChainRunner<TCommand>? _runner;
    
    public ChainBuilder<TCommand> Add(IChainProcessor<TCommand> processor)
    {
        _chain.Add(processor);
        
        return this;
    }

    public ChainRunner<TCommand> Build(IServiceProvider sp)
    {
        _runner ??= new ChainRunner<TCommand>(_chain, sp.GetRequiredService<ILogger<ChainRunner<TCommand>>>());
        
        return _runner;
    }
}