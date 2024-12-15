using Botticelli.Bot.Interfaces.Processors;
using Botticelli.Framework.Commands;
using Botticelli.Framework.Commands.Processors;
using Botticelli.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Botticelli.Framework.Extensions;

public class CommandRegisterServices<TCommand, TBot>(IServiceProvider sp)
    where TCommand : ICommand where TBot : IBot<TBot>
{
    public CommandRegisterServices<TCommand, TBot> RegisterProcessor<TCommandProcessor>()
        where TCommandProcessor : class, ICommandProcessor
    {
        sp.GetRequiredService<ClientProcessorFactory>()
            .AddProcessor<TCommandProcessor, TBot>(sp);

        return this;
    }
}