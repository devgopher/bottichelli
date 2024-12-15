using Botticelli.Scheduler.Settings;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Botticelli.Scheduler.Hangfire.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds a scheduler server (Hangfire)
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="config">Settings</param>
    /// <returns></returns>
    public static IServiceCollection AddHangfireScheduler(this IServiceCollection services,
        IConfiguration config)
    {
        var settings = new SchedulerSettings();
        config.GetSection(nameof(SchedulerSettings)).Bind(settings);

        return services.AddHangfire(cfg => cfg
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseActivator(new ContainerJobActivator(services))
                .UseMemoryStorage())
            .AddHangfireServer()
            .AddSingleton(services);
    }
}