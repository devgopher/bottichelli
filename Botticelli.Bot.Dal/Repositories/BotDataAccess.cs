using Microsoft.EntityFrameworkCore;

namespace Botticelli.Bot.Data.Repositories;

public class BotDataAccess(BotInfoContext context) : IBotDataAccess
{
    public void SetData(BotData.Entities.Bot.BotData data) => context.BotInfos.Upsert(data).Run();

    public BotData.Entities.Bot.BotData? GetData() => context.BotInfos.FirstOrDefault(); 
}