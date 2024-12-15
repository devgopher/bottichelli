using Botticelli.Scheduler.Interfaces;
using Botticelli.Scheduler.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Botticelli.Schedule.Quartz.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds a scheduler server (Quartz)
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="config">Settings</param>
    /// <returns></returns>
    public static IServiceCollection AddQuartzScheduler(this IServiceCollection services,
        IConfiguration config)
    {
        var settings = new SchedulerSettings();
        config.GetSection(nameof(SchedulerSettings)).Bind(settings);

        services.AddQuartz(q => q.UseSimpleTypeLoader())
            .AddSingleton<IJobManager, QuartzJobManager>();

        return services.AddQuartzHostedService(opt => opt.WaitForJobsToComplete = true);
    }
}