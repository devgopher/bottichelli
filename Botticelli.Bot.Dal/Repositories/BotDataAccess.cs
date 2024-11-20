using Botticelli.Bot.Data.Entities.Bot;
using Microsoft.EntityFrameworkCore;

namespace Botticelli.Bot.Data.Repositories;

public class BotDataAccess : IBotDataAccess
{
    private readonly BotInfoContext _context;

    public BotDataAccess(BotInfoContext context) => _context = context;

    public void SetData(BotData? data) => _context.BotInfos.Upsert(data).Run();

    public BotData? GetData() => _context.BotInfos.FirstOrDefault(); 
}