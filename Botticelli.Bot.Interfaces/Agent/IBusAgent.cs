using Botticelli.Bot.Interfaces.Bus.Handlers;
using Botticelli.Shared.API.Client.Requests;
using Botticelli.Shared.API.Client.Responses;
using Microsoft.Extensions.Hosting;

namespace Botticelli.Bot.Interfaces.Agent;

/// <summary>
///     Bus agent works on the side of endpoint
/// </summary>
public interface IBusAgent : IHostedService
{
    /// <summary>
    ///     Subscribes to a message queue
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task Subscribe(CancellationToken token);

    /// <summary>
    ///     Returns response from a target system to a bus
    /// </summary>
    /// <param name="response"></param>
    /// <param name="token"></param>
    /// <param name="timeoutMs"></param>
    /// <returns></returns>
    public Task SendResponseAsync(SendMessageResponse response,
        CancellationToken token,
        int timeoutMs = 10000);
}

public interface IBotticelliBusAgent<in THandler> : IBusAgent
    where THandler : IHandler<SendMessageRequest, SendMessageResponse>
{
}