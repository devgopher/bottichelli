using Botticelli.Shared.API.Admin.Responses;
using Botticelli.Shared.Constants;

namespace Botticelli.Server.Back.Services;

public interface IBotManagementService
{
    Task<bool> RegisterBot(string botId,
        string botKey,
        string botName,
        BotType botType,
        Dictionary<string, string> additionalParams = null);

    Task<bool> UpdateBot(string botId,
        string botKey,
        string botName,
        Dictionary<string, string> additionalParams = null);

    Task SetRequiredBotStatus(string botId, BotStatus status);
    Task SetKeepAlive(string botId);
    Task RemoveBot(string botId);
}