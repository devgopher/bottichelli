using Botticelli.BotData.Entities.Bot;
using Botticelli.Framework.Exceptions;
using Botticelli.Framework.Options;
using Botticelli.Interfaces;
using Botticelli.Shared.API.Admin.Requests;
using Botticelli.Shared.API.Admin.Responses;
using Botticelli.Shared.API.Client.Requests;
using Botticelli.Shared.API.Client.Responses;
using Microsoft.Extensions.Logging;
using Polly;

namespace Botticelli.Framework;

public class BotStatusService<TBot>(
    IHttpClientFactory httpClientFactory,
    ServerSettings serverSettings,
    TBot bot,
    ILogger<BotStatusService<TBot>> logger)
    : BotActualizationService<TBot>(httpClientFactory,
        serverSettings,
        bot,
        logger)
    where TBot : IBot
{
    private const short GetStatusPeriod = 5000;
    private readonly ManualResetEventSlim _getRequiredStatusEvent = new(false);
    private Task? _getRequiredStatusEventTask;

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        GetRequiredStatus(cancellationToken);

        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _getRequiredStatusEvent.Reset();

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Get required status for a bot from server
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="BotException"></exception>
    private void GetRequiredStatus(CancellationToken cancellationToken)
    {
        if (_getRequiredStatusEventTask != default) return;

        _getRequiredStatusEvent.Set();
        var request = new GetRequiredStatusFromServerRequest
        {
            BotId = BotId
        };

        _getRequiredStatusEventTask = Policy.HandleResult<GetRequiredStatusFromServerResponse>(r => true)
                                            .WaitAndRetryForeverAsync(_ => TimeSpan.FromMilliseconds(GetStatusPeriod))
                                            .ExecuteAndCaptureAsync(ct => Process(request, ct)!,
                                                                    cancellationToken);
    }

    private Task<GetRequiredStatusFromServerResponse?> Process(GetRequiredStatusFromServerRequest request, CancellationToken cancellationToken)
    {
        var task = InnerSend<GetRequiredStatusFromServerRequest, GetRequiredStatusFromServerResponse>(request,
                                                                                                      "/bot/client/GetRequiredBotStatus",
                                                                                                      cancellationToken);

        task.Wait(cancellationToken);

        var taskResult = task.Result;
        if (taskResult == default)
            throw new BotException("No result from server!");
        
        var botContext = taskResult.BotContext;
        if (botContext == default)
            throw new BotException("No bot context from server!");
        
        var botData = new BotData.Entities.Bot.BotData
        {
            BotId = botContext.BotId,
            Status = task.Result?.Status,
            BotKey = botContext.BotKey,
            AdditionalInfo = botContext.Items?.Select(it => new BotAdditionalInfo
                {
                    BotId = taskResult!.BotId,
                    ItemName = it.Key,
                    ItemValue = it.Value
                })
                .ToList()
        };

        Bot.SetBotContext(botData, cancellationToken);

        if (task.Exception != default)
        {
            Logger.LogError($"GetRequiredStatus task error: {task.Exception?.Message}");
            Bot.StopBotAsync(StopBotRequest.GetInstance(), cancellationToken);

            return task;
        }

        switch (task.Result?.Status)
        {
            case BotStatus.Unlocked:
                Bot.StartBotAsync(StartBotRequest.GetInstance(), cancellationToken);

                break;
            case BotStatus.Locked:
            case BotStatus.Unknown:
            case null:
                Bot.StopBotAsync(StopBotRequest.GetInstance(), cancellationToken);

                break;
            default: throw new ArgumentOutOfRangeException();
        }

        return task;
    }
}