using Botticelli.Server.Data;
using Botticelli.Server.Data.Entities.Bot.Broadcasting;
using Microsoft.EntityFrameworkCore;

namespace Botticelli.Server.Back.Services.Broadcasting;

/// <summary>
///     This class is intended for broadcasting
/// </summary>
public class BroadcastService(ServerDataContext context) : IBroadcastService
{
    public async Task BroadcastMessage(Broadcast message)
    {
        context.BroadcastMessages.Add(message);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Broadcast>> GetMessages(string botId)
        => await context.BroadcastMessages.Where(m => m.BotId.Equals(botId)).ToArrayAsync();

    public async Task DeleteReceived(string botId, string messageId)
    {
        context.BroadcastMessages.RemoveRange(
            context.BroadcastMessages.Where(bm => bm.BotId == botId && bm.Id == messageId));
        await context.SaveChangesAsync();
    }
}