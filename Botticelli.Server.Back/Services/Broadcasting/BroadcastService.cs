using Botticelli.Server.Data;
using Botticelli.Server.Data.Entities.Bot.Broadcasting;
using Microsoft.EntityFrameworkCore;

namespace Botticelli.Server.Back.Services.Broadcasting;

/// <summary>
///     This class is intended for broadcasting
/// </summary>
public class BroadcastService : IBroadcastService
{
    private readonly ServerDataContext _context;

    public BroadcastService(ServerDataContext context) => _context = context;

    public async Task BroadcastMessage(Broadcast message)
    {
        _context.BroadcastMessages.Add(message);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Broadcast>> GetMessages(string botId) 
        => await _context.BroadcastMessages.Where(m => m.BotId.Equals(botId)).ToArrayAsync();

    public async Task DeleteReceived(string botId) => _context.BroadcastMessages.RemoveRange(await GetMessages(botId));
}