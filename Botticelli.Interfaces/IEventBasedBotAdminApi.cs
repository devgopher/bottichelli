using Botticelli.Bot.Data.Entities.Bot;
using Botticelli.Shared.API.Admin.Requests;
using Botticelli.Shared.API.Admin.Responses;
using Botticelli.Shared.ValueObjects;

namespace Botticelli.Interfaces;

/// <summary>
///     Administration API
/// </summary>
public interface IEventBasedBotAdminApi
{
    /// <summary>
    ///     Starts serving
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task<StartBotResponse> StartBotAsync(StartBotRequest request, CancellationToken token);

    /// <summary>
    ///     Stops serving
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task<StopBotResponse> StopBotAsync(StopBotRequest request, CancellationToken token);

    /// <summary>
    ///     Sets bot context
    /// </summary>
    /// <param name="context"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task SetBotContext(BotData? context, CancellationToken token);
}