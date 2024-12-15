using Botticelli.Server.Data.Entities.Bot.Broadcasting;

namespace Botticelli.Server.Back.Services.Broadcasting;

/// <summary>
///     Broadcasting service.
///     A lifecycle of a broadcasted message: create => receive (poll model) => delete when received
/// </summary>
public interface IBroadcastService
{
    /// <summary>
    ///     Sends a message
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public Task BroadcastMessage(Broadcast message);

    /// <summary>
    ///     Gets messages for a bot
    /// </summary>
    /// <param name="botId"></param>
    /// <returns></returns>
    public Task<IEnumerable<Broadcast>> GetMessages(string botId);

    /// <summary>
    ///     Sets message as received and deletes it from a table
    /// </summary>
    /// <param name="botId"></param>
    /// <param name="messageId"></param>
    /// <returns></returns>
    public Task DeleteReceived(string botId, string messageId);
}