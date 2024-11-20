using System.Globalization;
using Botticelli.Bot.Data.Entities.Bot;
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

    public DbSet<BotData?> BotInfos { get; set; }
    public DbSet<BotAdditionalInfo> BotAdditionalInfos { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.Entity<BotData>();
}