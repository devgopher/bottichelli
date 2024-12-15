using Botticelli.BotData.Entities.Bot.Broadcasting;

namespace Botticelli.Bot.Data.Repositories;

/// <summary>
///     Broadcasting support DAL
/// </summary>
public interface IBroadCasting
{
    public void UpsertChat(Chat chat);
    public IEnumerable<Chat> GetChats(Func<Chat, bool> predicate);
}