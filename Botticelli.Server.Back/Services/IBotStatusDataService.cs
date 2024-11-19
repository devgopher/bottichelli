using Botticelli.Server.Data.Entities.Bot;
using Botticelli.Shared.API.Admin.Responses;

namespace Botticelli.Server.Back.Services;

/// <summary>
///     This interface is intended for bot management purposes (Getting a bots list/context/status)
/// </summary>
public interface IBotStatusDataService
{
    /// <summary>
    ///     Gets a collection of existing bots
    /// </summary>
    /// <returns></returns>
    ICollection<BotInfo> GetBots();

    /// <summary>
    ///     Gets a bot required status for answering on a poll request from a bot
    /// </summary>
    /// <param name="botId"></param>
    /// <returns></returns>
    Task<BotStatus?> GetRequiredBotStatus(string botId);

    /// <summary>
    ///     Gets bot key/token for a messenger (temporary here!)
    /// </summary>
    /// <param name="botId"></param>
    /// <returns></returns>
    Task<string> GetRequiredBotKey(string botId);

    /// <summary>
    ///     Gets bot context
    /// </summary>
    /// <param name="botId"></param>
    /// <returns></returns>
    Task<BotInfo> GetBotInfo(string botId);
}