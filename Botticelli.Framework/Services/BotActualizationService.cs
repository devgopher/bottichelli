using System.Net.Http.Json;
using System.Text.Json;
using Botticelli.Bot.Utils;
using Botticelli.Framework.Options;
using Botticelli.Interfaces;
using Flurl;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Botticelli.Framework.Services;

/// <summary>
///     This service is intended for sending keepalive/hello messages
///     to Botticelli Admin server and receiving status messages from it
/// </summary>
public abstract class BotActualizationService<TBot> : IHostedService
    where TBot : IBot
{
    protected readonly TBot Bot;
    protected readonly string? BotId = BotDataUtils.GetBotId();
    protected readonly ILogger Logger;
    protected readonly ManualResetEventSlim ActualizationEvent = new(false);
    protected readonly IHttpClientFactory HttpClientFactory;
    protected readonly ServerSettings ServerSettings;

    /// <summary>
    ///     This service is intended for sending keepalive/hello messages
    ///     to Botticelli Admin server and receiving status messages from it
    /// </summary>
    protected BotActualizationService(IHttpClientFactory httpClientFactory,
        ServerSettings serverSettings,
        TBot bot,
        ILogger logger)
    {
        HttpClientFactory = httpClientFactory;
        ServerSettings = serverSettings;
        Bot = bot;
        Logger = logger;
        
        ActualizationEvent.Reset();
    }

    public abstract Task StartAsync(CancellationToken cancellationToken);

    public virtual Task StopAsync(CancellationToken cancellationToken)
    {
        ActualizationEvent.Reset();

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Inner send
    /// </summary>
    /// <typeparam name="TReq">Request</typeparam>
    /// <typeparam name="TResp">Response</typeparam>
    /// <param name="request">Request</param>
    /// <param name="funcName">Response</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    protected  virtual async Task<TResp?> InnerSend<TReq, TResp>(TReq request,
        string funcName,
        CancellationToken cancellationToken)
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
}