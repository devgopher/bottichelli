using Botticelli.Server.Analytics.Cache;
using Botticelli.Server.Analytics.Services;
using Botticelli.Server.Analytics.Settings;

namespace Botticelli.Server.Analytics.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAnalytics(this IServiceCollection services, AnalyticsServerSettings serverSettings) =>
        services.AddScoped<MetricsReaderWriter>()
            .AddScoped<IMetricsInputService, MetricsInputService>()
            .AddScoped<IMetricsOutputService, MetricsOutputService>()
            .AddScoped<ICacheAccessor, LocalCacheAccessor>(_ => new LocalCacheAccessor(serverSettings.MaxCacheSize));
}