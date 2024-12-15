using Botticelli.Framework.Exceptions;
using Botticelli.Framework.Options;
using Botticelli.Interfaces;
using Botticelli.Shared.API.Client.Requests;
using Botticelli.Shared.API.Client.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;

namespace Botticelli.Framework;

public class BotKeepAliveService<TBot> : BotActualizationService<TBot> where TBot : IBot
{
    private const short KeepAlivePeriod = 5000;
    private readonly ManualResetEventSlim _getRequiredStatusEvent = new(false);
    private readonly ManualResetEventSlim _keepAliveEvent = new(false);
    private Task _keepAliveTask;

    public BotKeepAliveService(IHttpClientFactory httpClientFactory,
                               ServerSettings serverSettings,
                               TBot bot,
                               ILogger<BotActualizationService<TBot>> logger) : base(httpClientFactory,
                                                                                     serverSettings,
                                                                                     bot,
                                                                                     logger)

    {
        _keepAliveEvent.Reset();
        _getRequiredStatusEvent.Reset();
    }


    public override Task StartAsync(CancellationToken cancellationToken)
    {
        KeepAlive(cancellationToken);

        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _keepAliveEvent.Reset();

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Sends KeepAlive messages
    /// </summary>
    /// <param name="cancellationToken" />
    /// <exception cref="BotException" />
    private void KeepAlive(CancellationToken cancellationToken)
    {
        if (_keepAliveTask != default) return;

        _keepAliveEvent.Set();
        var request = new KeepAliveNotificationRequest
        {
            BotId = BotId
        };

        Logger.LogDebug($"KeepAlive botId: {BotId}");

        _keepAliveTask = Policy.HandleResult<KeepAliveNotificationResponse>(_ => true)
                               .WaitAndRetryForeverAsync(_ => TimeSpan.FromMilliseconds(KeepAlivePeriod))
                               .ExecuteAndCaptureAsync(ct => Process(request, ct),
                                                       cancellationToken);

        if (!_keepAliveTask.IsFaulted)
        {
            Logger.LogTrace($"{nameof(KeepAlive)} sent for bot: {BotId}");

            return;
        }

        Logger.LogError($"KeepAlive error: {_keepAliveTask.Exception?.Message}");

        throw new BotException($"{nameof(KeepAlive)} exception: {_keepAliveTask.Exception?.Message}",
                               _keepAliveTask.Exception);
    }

    private async Task<KeepAliveNotificationResponse?> Process(KeepAliveNotificationRequest request, CancellationToken ct)
    {
        try
        {
            var response = await InnerSend<KeepAliveNotificationRequest, KeepAliveNotificationResponse>(request,
                "/bot/client/KeepAlive",
                ct);

            Logger.LogDebug($"KeepAlive botId: {BotId} response: {response.BotId}, {response.IsSuccess}");

            return response;
        }
        catch (Exception ex)
        {
            return new KeepAliveNotificationResponse
            {
                IsSuccess = false,
                BotId = request.ToString()
            };
        }
    } 
}