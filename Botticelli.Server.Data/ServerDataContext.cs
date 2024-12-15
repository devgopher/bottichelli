using System.Globalization;
using Botticelli.Server.Data.Entities.Bot;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Botticelli.Server.Data;

public class ServerDataContext : DbContext
{
    // public ServerDataContext() : base((new DbContextOptionsBuilder<ServerDataContext>().UseSqlite("Data Source=database.db")).Options)
    // {
    //
    // }
    public ServerDataContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<BotInfo> BotInfos { get; set; }
    public DbSet<IdentityRole<string>> ApplicationRoles { get; set; }
    public DbSet<IdentityUserRole<string>> ApplicationUserRoles { get; set; }
    public DbSet<IdentityUser<string>> ApplicationUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BotInfo>();
        modelBuilder.Entity<IdentityRole<string>>()
            .HasKey(k => k.Id);
        modelBuilder.Entity<IdentityUserRole<string>>()
            .HasKey(k => new { k.UserId, k.RoleId });
        modelBuilder.Entity<IdentityUser<string>>()
            .HasKey(k => k.Id);

        modelBuilder.Entity<IdentityRole<string>>()
            .HasData(new IdentityRole<string>()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)
            }, new IdentityRole<string>
            {
                Id = Guid.NewGuid().ToString(),
                Name = "bot_manager",
                NormalizedName = "BOT_MANAGER",
                ConcurrencyStamp = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)
            }, new IdentityRole<string>
            {
                Id = Guid.NewGuid().ToString(),
                Name = "viewer",
                NormalizedName = "VIEWER",
                ConcurrencyStamp = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)
            });
    }
}