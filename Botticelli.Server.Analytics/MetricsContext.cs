using Botticelli.Server.Analytics.Cache;
using Botticelli.Server.Analytics.Models;
using Microsoft.EntityFrameworkCore;

namespace Botticelli.Server.Analytics;

public class MetricsContext : DbContext
{
    private readonly ICacheAccessor _cacheAccessor;

    public MetricsContext(DbContextOptions options, ICacheAccessor cacheAccessor) : base(options)
    {
        _cacheAccessor = cacheAccessor;
        ChangeTracker.DetectingEntityChanges += (sender, args) =>
        {
            if (args.Entry.Entity is not MetricModel model)
                return;

            switch (args.Entry.State)
            {
                case EntityState.Deleted:
                    _cacheAccessor.Remove(model);
                    break;
                case EntityState.Added:
                    _cacheAccessor.Set(model);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        };
    }

    public DbSet<MetricModel> MetricModels { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MetricModel>();
    }
}