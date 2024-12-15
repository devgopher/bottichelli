using Botticelli.Server.Data;
using Botticelli.Server.Data.Entities.Bot;
using Botticelli.Shared.API.Admin.Responses;

namespace Botticelli.Server.Back.Services;

/// <summary>
///     This class is intended for bot management purposes (Getting a bots list/context/status)
/// </summary>
public class BotStatusDataService : IBotStatusDataService
{
    private readonly ServerDataContext _context;

    public BotStatusDataService(ServerDataContext context) => _context = context;

    public ICollection<BotInfo> GetBots() => _context.BotInfos.ToArray();

    /// <summary>
    ///     Gets a bot required status for answering on a poll request from a bot
    /// </summary>
    /// <param name="botId"></param>
    /// <returns></returns>
    public async Task<BotStatus?> GetRequiredBotStatus(string botId)
        => _context.BotInfos.FirstOrDefault(b => b.BotId == botId)?.Status ?? BotStatus.Unknown;

    [Obsolete("Use GetRequiredBotContext")]
    public async Task<string> GetRequiredBotKey(string botId) => _context.BotInfos.FirstOrDefault(bi => bi.BotId == botId)?.BotKey ?? string.Empty;

    public async Task<BotInfo> GetBotInfo(string botId) => _context.BotInfos.FirstOrDefault(bi => bi.BotId == botId);
}