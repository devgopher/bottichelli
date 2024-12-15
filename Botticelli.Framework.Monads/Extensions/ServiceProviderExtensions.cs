using Botticelli.Framework.Extensions;
using Botticelli.Framework.Monads.Commands.Context;
using Botticelli.Framework.Monads.Commands.Processors;
using Botticelli.Interfaces;

namespace Botticelli.Framework.Monads.Extensions;

public static class ServiceProviderExtensions
{
    public static IServiceProvider UseMonadsChain<TCommand, TBot>(this IServiceProvider sp)
        where TCommand : class, IChainCommand, new()
        where TBot : IBot<TBot>
    {
        sp.RegisterBotCommand<TCommand, ChainRunProcessor<TCommand>, TBot>();

        return sp;
    }
}