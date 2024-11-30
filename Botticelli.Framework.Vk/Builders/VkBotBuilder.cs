using Botticelli.Audio;
using Botticelli.Bot.Data;
using Botticelli.Bot.Data.Repositories;
using Botticelli.Bot.Data.Settings;
using Botticelli.Bot.Utils;
using Botticelli.Bot.Utils.TextUtils;
using Botticelli.Client.Analytics;
using Botticelli.Client.Analytics.Settings;
using Botticelli.Framework.Builders;
using Botticelli.Framework.Extensions;
using Botticelli.Framework.Options;
using Botticelli.Framework.Security;
using Botticelli.Framework.Services;
using Botticelli.Framework.Vk.Messages.Handlers;
using Botticelli.Framework.Vk.Messages.HostedService;
using Botticelli.Framework.Vk.Messages.Options;
using Botticelli.Framework.Vk.Messages.Utils;
using Botticelli.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Botticelli.Framework.Vk.Messages.Builders;

public class VkBotBuilder : BotBuilder<VkBotBuilder, VkBot>
{
    private LongPollMessagesProviderBuilder? _longPollMessagesProviderBuilder;
    private LongPollMessagesProvider? _longPollMessagesProvider;
    private MessagePublisherBuilder? _messagePublisherBuilder;
    private MessagePublisher? _messagePublisher;
    private VkStorageUploaderBuilder? _vkStorageUploaderBuilder;
    private VkStorageUploader? _vkStorageUploader;
    
    private VkBotSettings? BotSettings { get; set; }
    
    protected override VkBot InnerBuild()
    {
        Services!.AddHttpClient<BotStatusService<VkBot>>()
                 .AddServerCertificates(BotSettings);
        Services!.AddHostedService<BotStatusService<IBot<VkBot>>>();
        Services!.AddHttpClient<BotKeepAliveService<VkBot>>()
                 .AddServerCertificates(BotSettings);
        Services!.AddHttpClient<GetBroadCastMessagesService<VkBot>>()
            .AddServerCertificates(BotSettings);
        Services!.AddHostedService<BotKeepAliveService<IBot<VkBot>>>();
        Services!.AddHostedService<GetBroadCastMessagesService<IBot<VkBot>>>();

        Services!.AddHostedService<VkBotHostedService>();
        var botId = BotDataUtils.GetBotId();

        if (botId == null) throw new InvalidDataException($"{nameof(botId)} shouldn't be null!");

        #region Metrics

        var metricsPublisher = new MetricsPublisher(AnalyticsClientSettingsBuilder.Build());
        var metricsProcessor = new MetricsProcessor(metricsPublisher);
        Services!.AddSingleton(metricsPublisher);
        Services!.AddSingleton(metricsProcessor);

        #endregion

        #region Data

        Services!.AddDbContext<BotInfoContext>(o => o.UseSqlite($"Data source={BotDataAccessSettingsBuilder.Build().ConnectionString}"));
        Services!.AddScoped<IBotDataAccess, BotDataAccess>();

        #endregion

        #region TextTransformer

        Services!.AddTransient<ITextTransformer, VkTextTransformer>();

        #endregion

        _longPollMessagesProvider = _longPollMessagesProviderBuilder.Build();
        _messagePublisher = _messagePublisherBuilder.Build();
        _vkStorageUploader = _vkStorageUploaderBuilder.Build();
        
        Services!.AddBotticelliFramework()
                 .AddSingleton<IBotUpdateHandler, BotUpdateHandler>();

        var sp = Services!.BuildServiceProvider();

        return new VkBot(_longPollMessagesProvider,
                         _messagePublisher,
                        _vkStorageUploader,
                         sp.GetRequiredService<IBotDataAccess>(),
                         sp.GetRequiredService<IBotUpdateHandler>(),
                         sp.GetRequiredService<MetricsProcessor>(),
                         sp.GetRequiredService<ILogger<VkBot>>());
    }

    public override VkBotBuilder AddBotSettings<TBotSettings>(BotSettingsBuilder<TBotSettings> settingsBuilder)
    {
        BotSettings = settingsBuilder.Build() as VkBotSettings ?? throw new InvalidOperationException();
        
        return this;
    }
    
    public VkBotBuilder AddClient(LongPollMessagesProviderBuilder builder)
    {
        _longPollMessagesProviderBuilder = builder;

        return this;
    }

    public static VkBotBuilder Instance(IServiceCollection services,
                                        ServerSettingsBuilder<ServerSettings> serverSettingsBuilder,
                                        BotSettingsBuilder<VkBotSettings> settingsBuilder,
                                        DataAccessSettingsBuilder<DataAccessSettings> dataAccessSettingsBuilder,
                                        AnalyticsClientSettingsBuilder<AnalyticsClientSettings> analyticsClientSettingsBuilder)
    {
        return new VkBotBuilder()
               .AddServices(services)
               .AddServerSettings(serverSettingsBuilder)
               .AddAnalyticsSettings(analyticsClientSettingsBuilder)
               .AddBotDataAccessSettings(dataAccessSettingsBuilder)
               .AddBotSettings(settingsBuilder);
    }
}