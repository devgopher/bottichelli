using Botticelli.Server.Data;
using Botticelli.Server.Data.Entities.Bot;
using Botticelli.Shared.API.Admin.Responses;

namespace Botticelli.Server.Back.Services;

/// <summary>
///     This class is intended for bot management purposes (Getting a bots list/context/status)
/// </summary>
public class BotStatusDataService(ServerDataContext context) : IBotStatusDataService
{
    public ICollection<BotInfo> GetBots() => context.BotInfos.ToArray();

    /// <summary>
    ///     Gets a bot required status for answering on a poll request from a bot
    /// </summary>
    /// <param name="botId"></param>
    /// <returns></returns>
    public Task<BotStatus?> GetRequiredBotStatus(string botId)
        => Task.FromResult<BotStatus?>(context.BotInfos.FirstOrDefault(b => b.BotId == botId)?.Status ??
                                       BotStatus.Unknown);

    [Obsolete("Use GetRequiredBotContext")]
    public Task<string> GetRequiredBotKey(string botId) =>
        Task.FromResult(context.BotInfos.FirstOrDefault(bi => bi.BotId == botId)?.BotKey ?? string.Empty);

    public Task<BotInfo?> GetBotInfo(string botId) =>
        Task.FromResult(context.BotInfos.FirstOrDefault(bi => bi.BotId == botId));
}