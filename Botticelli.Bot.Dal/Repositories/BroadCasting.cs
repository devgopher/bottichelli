using Botticelli.BotData.Entities.Bot.Broadcasting;
using Microsoft.EntityFrameworkCore;

namespace Botticelli.Bot.Data.Repositories;

public class BroadCasting(BotInfoContext context) : IBroadCasting
{
    public void UpsertChat(Chat chat) => context.Upsert(chat);

    public IEnumerable<Chat> GetChats(Func<Chat, bool> predicate) =>
        context.Set<Chat>().Where(predicate).AsEnumerable();
}