using Microsoft.Extensions.DependencyInjection;

namespace Botticelli.Framework.Commands.Processors;

public class CommandChainProcessorBuilder<TInputCommand> where TInputCommand : class, ICommand
{
    private readonly IServiceCollection _services;
    private readonly List<Type> _typesChain = new(3);
    private ICommandChainProcessor<TInputCommand>? _chainProcessor;

    public CommandChainProcessorBuilder(IServiceCollection services)
    {
        _services = services;

        _typesChain.Add(typeof(CommandChainFirstElementProcessor<TInputCommand>));
        _services.AddScoped<CommandChainFirstElementProcessor<TInputCommand>>();
    }

    public CommandChainProcessorBuilder<TInputCommand> AddNext<TNextProcessor>()
        where TNextProcessor : class, ICommandChainProcessor<TInputCommand>
    {
        _typesChain.Add(typeof(TNextProcessor));
        _services.AddScoped<TNextProcessor>();

        return this;
    }

    public ICommandChainProcessor<TInputCommand>? Build()
    {
        if (_typesChain.Count == 0) return null;

        // initializing chain processors...

        var sp = _services.BuildServiceProvider();

        _chainProcessor ??= sp.GetRequiredService(_typesChain.First()) as ICommandChainProcessor<TInputCommand>;

        // making a chain...
        var prev = _chainProcessor;
        foreach (var type in _typesChain.Skip(1))
        {
            var proc = sp.GetRequiredService(type) as ICommandChainProcessor<TInputCommand>;

            if (prev != null)
                prev.Next = proc;

            prev = proc;
        }

        return _chainProcessor;
    }
}