using Botticelli.Bot.Interfaces.Processors;
using Botticelli.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Botticelli.Framework.Commands.Processors;

public class ClientProcessorFactory
{
    private static readonly IList<IClientMessageProcessor?> ClientProcessors
        = new List<IClientMessageProcessor?>(10);

    private readonly Random _rnd = new(DateTime.Now.Millisecond);

    public void AddProcessor<TProcessor, TBot>(IServiceProvider sp)
        where TProcessor : class, IClientMessageProcessor
        where TBot : IBot<TBot>
    {
        var procCnt = ClientProcessors.Count(x => x is TProcessor);

        if (procCnt > 10) return;

        var bot = sp.GetRequiredService<IBot<TBot>>();
        var proc = sp.CreateScope().ServiceProvider.GetRequiredService<TProcessor>();
        proc.SetBot(bot);
        proc.SetServiceProvider(sp);
        ClientProcessors.Add(proc);
    }

    public void AddSingleProcessor<TBot>(IServiceProvider sp, ICommandProcessor? proc)
        where TBot : IBot<TBot>
    {
        var bot = sp.GetRequiredService<IBot<TBot>>();
        proc.SetBot(bot);
        proc.SetServiceProvider(sp);
        ClientProcessors.Add(proc);
    }


    public IEnumerable<IClientMessageProcessor> GetProcessors(bool excludeChain = true)
        => ClientProcessors.AsEnumerable()
            .Where(p => !excludeChain || !p.GetType().IsAssignableTo(typeof(ICommandChainProcessor)))
            .OrderBy(_ => _rnd.Next() % ClientProcessors.Count)
            .DistinctBy(p => p.GetType());


    public IEnumerable<ICommandChainProcessor> GetCommandChainProcessors() =>
        ClientProcessors.AsEnumerable()
            .Where(p => p.GetType().IsAssignableTo(typeof(ICommandChainFirstElementProcessor)))
            .Cast<ICommandChainFirstElementProcessor>()
            .DistinctBy(p => p.GetType());
}