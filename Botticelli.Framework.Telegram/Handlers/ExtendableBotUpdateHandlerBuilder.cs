using System.Collections.Immutable;
using Botticelli.Framework.Exceptions;
using Botticelli.Shared.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace Botticelli.Framework.Telegram.Handlers;

public class ExtendableBotUpdateHandlerBuilder
{
    private readonly IList<IBotUpdateHandler> _handlers = new List<IBotUpdateHandler>(5);
    private readonly IServiceProvider _serviceProvider;

    public ExtendableBotUpdateHandlerBuilder(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    public ExtendableBotUpdateHandlerBuilder AddHandler(IBotUpdateHandler handler)
    {
        handler.NotNull();
        
        if (_handlers.Contains(handler) || _handlers.Any(h => h.GetType() == handler.GetType()))
            throw new BotLoadingException($"An handler with type {handler.GetType().FullName} already exists!");
        
        _handlers.Add(handler);
        
        return this;
    }

    public ExtendableBotUpdateHandlerBuilder AddHandler<T>()
        where T : class, IBotUpdateHandler
    {
        var handler = _serviceProvider.GetRequiredService<T>();
        
        return AddHandler(handler);
    }

    public ExtendableBotUpdateHandler Build()
    {
        _handlers.NotNullOrEmpty();
        
        return new ExtendableBotUpdateHandler(_handlers.ToImmutableList());
    }

    public static ExtendableBotUpdateHandlerBuilder Instance(IServiceCollection? services) 
        => new(services.BuildServiceProvider());
}