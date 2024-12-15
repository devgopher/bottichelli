using Botticelli.Framework.Options;
using Botticelli.Interfaces;
using Botticelli.Shared.API.Client.Requests;
using Botticelli.Shared.API.Client.Responses;
using Botticelli.Shared.Utils;
using Microsoft.Extensions.Logging;

namespace Botticelli.Framework.Services;

/// <summary>
///     Receives broadcast message
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
    where TBot : IBot
{
    private TBot _bot = bot;

    protected override async Task InnerProcess(GetBroadCastMessagesResponse response, CancellationToken ct)
    {
        response.BotId.NotNull();
        response.Messages.NotNull();

        foreach (var message in response.Messages)
        {
            var sendMessageRequest = new SendMessageRequest { Message = message };

            await _bot.SendMessageAsync(sendMessageRequest, ct);
        }
    }
}