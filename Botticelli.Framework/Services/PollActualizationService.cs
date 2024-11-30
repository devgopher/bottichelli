using System.Net.Http.Json;
using Botticelli.Framework.Exceptions;
using Botticelli.Framework.Options;
using Botticelli.Interfaces;
using Botticelli.Shared.API.Client.Requests;
using Flurl;
using Microsoft.Extensions.Logging;
using Polly;

namespace Botticelli.Framework.Services;

public class PollActualizationService<TBot, TRequest, TResponse>(
    IHttpClientFactory httpClientFactory,
    string subPath,
    ServerSettings serverSettings,
    TBot bot,
    ILogger logger)
    : BotActualizationService<TBot>(httpClientFactory, serverSettings, bot, logger)
    where TBot : IBot
    where TRequest : IBotRequest, new()
{
    private const short ActionPeriod = 5000;
    private Task? _periodicTask;

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        ProcessRequest(cancellationToken);

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Inner send
    /// </summary>
    /// <typeparam name="TReq">Request</typeparam>
    /// <typeparam name="TResp">Response</typeparam>
    /// <param name="request">Request</param>
    /// <param name="funcName">Method on a server</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    protected override async Task<TResp?> InnerSend<TReq, TResp>(TReq request,
        string funcName,
        CancellationToken cancellationToken) where TResp : default
    {
        try
        {
            using var httpClient = HttpClientFactory.CreateClient();

            var content = JsonContent.Create(request);

            Logger.LogDebug("InnerSend request: {request}", request);

            var response = await httpClient.PostAsync(Url.Combine(ServerSettings.ServerUri, funcName),
                content,
                cancellationToken);

            return await response.Content.ReadFromJsonAsync<TResp>(cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
        }

        return default;
    }

    /// <summary>
    ///     Sends messages to a server
    /// </summary>
    /// <param name="cancellationToken" />
    /// <exception cref="BotException" />
    private void ProcessRequest(CancellationToken cancellationToken)
    {
        if (_periodicTask != default)
            return;

        ActualizationEvent.Set();
        var request = new TRequest
        {
            BotId = BotId
        };

        Logger.LogDebug("Poll botId: {botId}", BotId);

        _periodicTask = Policy.HandleResult<TResponse>(_ => true)
            .WaitAndRetryForeverAsync(_ => TimeSpan.FromMilliseconds(ActionPeriod))
            .ExecuteAndCaptureAsync(ct => Process(request, ct),
                cancellationToken);

        if (_periodicTask.IsFaulted)
            return;
        Logger.LogTrace("{where} sent for bot: {botId}", typeof(TRequest).Name, BotId);
    }

    private async Task<TResponse> Process(TRequest request, CancellationToken ct)
    {
        var response = await InnerSend<TRequest, TResponse>(request,
            $"/bot/client/{subPath}",
            ct);

        Logger.LogDebug("Poll botId: {botId} response: {response}", BotId, response);

        if (response != null)
            await InnerProcess(response, ct);
        
        return response!;
    }

    protected virtual Task InnerProcess(TResponse response, CancellationToken ct) => Task.CompletedTask;
}