using System.Configuration;
using Botticelli.Client.Analytics.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Botticelli.Client.Analytics.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAnalyticsClient(this IServiceCollection services,
        IConfiguration configuration)
        => services.AddAnalyticsClient<AnalyticsClientSettings>(configuration);
    
    public static IServiceCollection AddAnalyticsClient(this IServiceCollection services,
        Action<AnalyticsClientSettingsBuilder<AnalyticsClientSettings>> func)
        => services.AddAnalyticsClient<AnalyticsClientSettings>(func);
    
    public static IServiceCollection AddAnalyticsClient<T>(this IServiceCollection services, Action<AnalyticsClientSettingsBuilder<T>> func)
    where T: AnalyticsClientSettings, new()
    {
        var builder = new AnalyticsClientSettingsBuilder<T>();
        func(builder);

        return services.AddAnalyticsClient(builder.Build());
    }

    public static IServiceCollection AddAnalyticsClient<T>(this IServiceCollection services, IConfiguration configuration)
        where T: AnalyticsClientSettings, new()
    {
        var analyticsSettings = configuration
            .GetSection(typeof(T).Name)
            .Get<T>();
        if (analyticsSettings == null)
            throw new ConfigurationErrorsException($"No section for: {typeof(T)}!");

        return services.AddAnalyticsClient<T>(opt => opt.Set(analyticsSettings));
    }

    public static IServiceCollection AddAnalyticsClient(this IServiceCollection services,
        AnalyticsClientSettings clientSettings)
        => services.AddAnalyticsClient<AnalyticsClientSettings>(clientSettings);
    
    public static IServiceCollection AddAnalyticsClient<T>(this IServiceCollection services, T clientSettings)
        where T: AnalyticsClientSettings, new()
    {
        return services.AddSingleton<MetricsPublisher>()
            .AddSingleton(clientSettings)
            .AddMediatR(c => c.RegisterServicesFromAssembly(typeof(MetricsPublisher).Assembly))
            .AddSingleton<MetricsProcessor>();
    }
}