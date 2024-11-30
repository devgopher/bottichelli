using Botticelli.Bot.Data;
using Botticelli.Bot.Data.Repositories;
using Botticelli.Bot.Data.Settings;
using Botticelli.Bot.Utils;
using Botticelli.Bot.Utils.TextUtils;
using Botticelli.Client.Analytics;
using Botticelli.Client.Analytics.Settings;
using Botticelli.Framework.Builders;
using Botticelli.Framework.Controls.Parsers;
using Botticelli.Framework.Extensions;
using Botticelli.Framework.Options;
using Botticelli.Framework.Security;
using Botticelli.Framework.Services;
using Botticelli.Framework.Telegram.Decorators;
using Botticelli.Framework.Telegram.Handlers;
using Botticelli.Framework.Telegram.HostedService;
using Botticelli.Framework.Telegram.Layout;
using Botticelli.Framework.Telegram.Options;
using Botticelli.Framework.Telegram.Utils;
using Botticelli.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.ReplyMarkups;

namespace Botticelli.Framework.Telegram.Builders;

public class TelegramBotBuilder : BotBuilder<TelegramBotBuilder, TelegramBot>
{
    private TelegramClientDecoratorBuilder _builder = null!;
    private TelegramClientDecorator _client = null!;

    private TelegramBotSettings? BotSettings { get; set; }

    public static TelegramBotBuilder Instance(IServiceCollection services,
        ServerSettingsBuilder<ServerSettings> serverSettingsBuilder,
        BotSettingsBuilder<TelegramBotSettings> settingsBuilder,
        DataAccessSettingsBuilder<DataAccessSettings> dataAccessSettingsBuilder,
        AnalyticsClientSettingsBuilder<AnalyticsClientSettings> analyticsClientSettingsBuilder) =>
        new TelegramBotBuilder()
            .AddServices(services)
            .AddServerSettings(serverSettingsBuilder)
            .AddAnalyticsSettings(analyticsClientSettingsBuilder)
            .AddBotDataAccessSettings(dataAccessSettingsBuilder)
            .AddBotSettings(settingsBuilder);

    public TelegramBotBuilder AddClient(TelegramClientDecoratorBuilder builder)
    {
        _builder = builder;

        return this;
    }

    protected override TelegramBot InnerBuild()
    {
        Services!.AddSingleton(ServerSettingsBuilder.Build());

        Services!.AddHttpClient<BotStatusService<TelegramBot>>()
            .AddServerCertificates(BotSettings);
        Services!.AddHostedService<BotStatusService<IBot<TelegramBot>>>();

        Services!.AddHttpClient<BotKeepAliveService<TelegramBot>>()
            .AddServerCertificates(BotSettings);

        Services!.AddHostedService<BotKeepAliveService<IBot<TelegramBot>>>();

        Services!.AddHostedService<TelegramBotHostedService>();
        var botId = BotDataUtils.GetBotId();

        if (botId == null) throw new InvalidDataException($"{nameof(botId)} shouldn't be null!");

        #region Metrics

        var metricsPublisher = new MetricsPublisher(AnalyticsClientSettingsBuilder.Build());
        var metricsProcessor = new MetricsProcessor(metricsPublisher);
        Services!.AddSingleton(metricsPublisher);
        Services!.AddSingleton(metricsProcessor);

        #endregion

        #region Data

        Services!.AddDbContext<BotInfoContext>(o =>
            o.UseSqlite($"Data source={BotDataAccessSettingsBuilder.Build().ConnectionString}"));
        Services!.AddScoped<IBotDataAccess, BotDataAccess>();

        #endregion

        #region TextTransformer

        Services!.AddTransient<ITextTransformer, TelegramTextTransformer>();

        #endregion

        if (BotSettings?.UseThrottling is true) _builder.AddThrottler(new Throttler());
        _client = _builder.Build();
        _client.Timeout = TimeSpan.FromMilliseconds(BotSettings?.Timeout ?? 10000);

        Services!.AddScoped<ILayoutSupplier<IReplyMarkup>, ReplyTelegramLayoutSupplier>()
            .AddBotticelliFramework()
            .AddSingleton<IBotUpdateHandler, BotUpdateHandler>();

        Services!.AddSingleton(ServerSettingsBuilder.Build());

        var sp = Services!.BuildServiceProvider();
        ApplyMigrations(sp);
        
        return new TelegramBot(_client,
            sp.GetRequiredService<IBotUpdateHandler>(),
            sp.GetRequiredService<ILogger<TelegramBot>>(),
            sp.GetRequiredService<MetricsProcessor>(),
            sp.GetRequiredService<ITextTransformer>(),
            sp.GetRequiredService<IBotDataAccess>());
    }

    public override TelegramBotBuilder AddBotSettings<TBotSettings>(BotSettingsBuilder<TBotSettings> settingsBuilder)
    {
        BotSettings = settingsBuilder.Build() as TelegramBotSettings ?? throw new InvalidOperationException();

        return this;
    }

    private void ApplyMigrations(IServiceProvider sp) => sp.GetRequiredService<BotInfoContext>().Database.Migrate();
}