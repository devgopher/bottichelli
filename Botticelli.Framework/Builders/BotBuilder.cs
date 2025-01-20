using Botticelli.Bot.Data.Settings;
using Botticelli.Client.Analytics.Settings;
using Botticelli.Framework.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Botticelli.Framework.Builders;

public abstract class BotBuilder<TBot>
{
    protected abstract void Assert();

    public TBot? Build()
    {
        Assert();

        return InnerBuild();
    }

    protected abstract TBot? InnerBuild();
}

public abstract class BotBuilder<TBotBuilder, TBot> : BotBuilder<TBot>
    where TBotBuilder : BotBuilder<TBot>
{
    private readonly ServerSettings _serverSettings;
    protected AnalyticsClientSettingsBuilder<AnalyticsClientSettings> AnalyticsClientSettingsBuilder;
    protected DataAccessSettingsBuilder<DataAccessSettings> BotDataAccessSettingsBuilder;
    protected ServerSettingsBuilder<ServerSettings> ServerSettingsBuilder;
    protected IServiceCollection? Services;

    protected override void Assert()
    {
    }

    protected TBotBuilder AddServices(IServiceCollection services)
    {
        Services = services;

        return (this as TBotBuilder)!;
    }

    public abstract TBotBuilder AddBotSettings<TBotSettings>(BotSettingsBuilder<TBotSettings> settingsBuilder)
        where TBotSettings : BotSettings, new();

    public TBotBuilder AddAnalyticsSettings(
        AnalyticsClientSettingsBuilder<AnalyticsClientSettings> clientSettingsBuilder)
    {
        AnalyticsClientSettingsBuilder = clientSettingsBuilder;

        return (this as TBotBuilder)!;
    }

    public TBotBuilder AddServerSettings(ServerSettingsBuilder<ServerSettings> settingsBuilder)
    {
        ServerSettingsBuilder = settingsBuilder;

        return (this as TBotBuilder)!;
    }

    public TBotBuilder AddBotDataAccessSettings(DataAccessSettingsBuilder<DataAccessSettings> botDataAccessBuilder)
    {
        BotDataAccessSettingsBuilder = botDataAccessBuilder;

        return (this as TBotBuilder)!;
    }
}