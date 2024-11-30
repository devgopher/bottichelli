using Botticelli.Framework.Options;
using Botticelli.Interfaces;
using Botticelli.Shared.API.Client.Requests;
using Botticelli.Shared.API.Client.Responses;
using Microsoft.Extensions.Logging;

namespace Botticelli.Framework.Services;

public class BotKeepAliveService<TBot>(
    IHttpClientFactory httpClientFactory,
    ServerSettings serverSettings,
    TBot bot,
    ILogger<BotActualizationService<TBot>> logger)
    : PollActualizationService<TBot, KeepAliveNotificationRequest, KeepAliveNotificationResponse>(httpClientFactory,
        "keepalive",
        serverSettings,
        bot,
        logger)
    where TBot : IBot;