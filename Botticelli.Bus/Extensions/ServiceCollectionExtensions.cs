using Botticelli.Bot.Interfaces.Bus.Handlers;
using Botticelli.Bot.Interfaces.Client;
using Botticelli.Bus.None.Agent;
using Botticelli.Bus.None.Client;
using Botticelli.Interfaces;
using Botticelli.Shared.API.Client.Requests;
using Botticelli.Shared.API.Client.Responses;
using Microsoft.Extensions.DependencyInjection;

namespace Botticelli.Bus.None.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Uses a no-bus scheme
    /// </summary>
    /// <typeparam name="TBot"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection UsePassBusClient<TBot>(this IServiceCollection services)
        where TBot : IBot =>
        services.AddSingleton<IBusClient, PassClient>();

    /// <summary>
    ///     Uses an event-based-no-bus scheme
    /// </summary>
    /// <typeparam name="TBot"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection UsePassEventBusClient<TBot>(this IServiceCollection services)
        where TBot : IBot =>
        services.AddSingleton<IEventBusClient, PassEventClient>();

    /// <summary>
    ///     Uses a no-bus scheme
    /// </summary>
    /// <typeparam name="TBot"></typeparam>
    /// <typeparam name="THandler"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection UsePassBusAgent<TBot, THandler>(this IServiceCollection services)
        where TBot : IBot where THandler : IHandler<SendMessageRequest, SendMessageResponse> =>
        services.AddHostedService<PassAgent<THandler>>();
}