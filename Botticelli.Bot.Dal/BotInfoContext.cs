using Botticelli.BotData.Entities.Bot;
using Botticelli.BotData.Entities.Bot.Broadcasting;
using Microsoft.EntityFrameworkCore;

namespace Botticelli.Bot.Data;

public class BotInfoContext : DbContext
{
    // public BotInfoContext() : base((new DbContextOptionsBuilder<BotInfoContext>().UseSqlite("Data Source=database.db")).Options)
    // {
    //
    // }

    public BotInfoContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<BotData.Entities.Bot.BotData> BotInfos { get; set; }
    public DbSet<BotAdditionalInfo> BotAdditionalInfos { get; set; }
    public DbSet<Chat> Chats { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BotData.Entities.Bot.BotData>();
        modelBuilder.Entity<BotAdditionalInfo>();
        modelBuilder.Entity<Chat>().HasKey(c => new { c.ChatId, c.BotId });
    }
}