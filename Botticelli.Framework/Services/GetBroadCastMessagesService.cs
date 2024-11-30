using Botticelli.Framework.Options;
using Botticelli.Interfaces;
using Botticelli.Shared.API.Client.Requests;
using Botticelli.Shared.API.Client.Responses;
using Microsoft.Extensions.Logging;


namespace Botticelli.Framework.Services;

/// <summary>
/// Receives broadcast message
/// </summary>
/// <param name="httpClientFactory"></param>
/// <param name="serverSettings"></param>
/// <param name="bot"></param>
/// <param name="logger"></param>
/// <typeparam name="TBot"></typeparam>
public class GetBroadCastMessagesService<TBot>(
    IHttpClientFactory httpClientFactory,
    ServerSettings serverSettings,
    TBot bot,
    ILogger<BotActualizationService<TBot>> logger)
    : PollActualizationService<TBot, GetBroadCastMessagesRequest, GetBroadCastMessagesResponse>(httpClientFactory,
        "broadcast",
        serverSettings,
        bot,
        logger)
    where TBot : IBot;