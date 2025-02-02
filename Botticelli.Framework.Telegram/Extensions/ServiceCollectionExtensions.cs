﻿using System.Configuration;
using Botticelli.Bot.Data.Settings;
using Botticelli.Client.Analytics.Settings;
using Botticelli.Framework.Controls.Parsers;
using Botticelli.Framework.Options;
using Botticelli.Framework.Telegram.Builders;
using Botticelli.Framework.Telegram.Decorators;
using Botticelli.Framework.Telegram.Layout;
using Botticelli.Framework.Telegram.Options;
using Botticelli.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types.ReplyMarkups;

namespace Botticelli.Framework.Telegram.Extensions;

public static class ServiceCollectionExtensions
{
    private static readonly BotSettingsBuilder<TelegramBotSettings> SettingsBuilder = new();
    private static readonly ServerSettingsBuilder<ServerSettings> ServerSettingsBuilder = new();
    private static readonly AnalyticsClientSettingsBuilder<AnalyticsClientSettings> AnalyticsClientOptionsBuilder = new();
    private static readonly DataAccessSettingsBuilder<DataAccessSettings> DataAccessSettingsBuilder = new();

    public static IServiceCollection AddTelegramBot(this IServiceCollection services, IConfiguration configuration)
    {
        var telegramBotSettings = configuration
                                  .GetSection(TelegramBotSettings.Section)
                                  .Get<TelegramBotSettings>() ??
                                  throw new ConfigurationErrorsException($"Can't load configuration for {nameof(TelegramBotSettings)}!");

        var analyticsClientSettings = configuration
                                      .GetSection(AnalyticsClientSettings.Section)
                                      .Get<AnalyticsClientSettings>() ??
                                      throw new ConfigurationErrorsException($"Can't load configuration for {nameof(AnalyticsClientSettings)}!");

        var serverSettings = configuration
                             .GetSection(ServerSettings.Section)
                             .Get<ServerSettings>() ??
                             throw new ConfigurationErrorsException($"Can't load configuration for {nameof(ServerSettings)}!");

        var dataAccessSettings = configuration
                                 .GetSection(DataAccessSettings.Section)
                                 .Get<DataAccessSettings>() ??
                                 throw new ConfigurationErrorsException($"Can't load configuration for {nameof(DataAccessSettings)}!");

        return services.AddTelegramBot(telegramBotSettings,
                                       analyticsClientSettings,
                                       serverSettings,
                                       dataAccessSettings);
    }

    public static IServiceCollection AddTelegramBot(this IServiceCollection services,
                                                    TelegramBotSettings botSettings,
                                                    AnalyticsClientSettings analyticsClientSettings,
                                                    ServerSettings serverSettings,
                                                    DataAccessSettings dataAccessSettings) =>
            services.AddTelegramBot(o => o.Set(botSettings),
                                    o => o.Set(analyticsClientSettings),
                                    o => o.Set(serverSettings),
                                    o => o.Set(dataAccessSettings));

    /// <summary>
    ///     Adds a Telegram bot
    /// </summary>
    /// <param name="services"></param>
    /// <param name="optionsBuilderFunc"></param>
    /// <param name="analyticsOptionsBuilderFunc"></param>
    /// <param name="serverSettingsBuilderFunc"></param>
    /// <param name="dataAccessSettingsBuilderFunc"></param>
    /// <returns></returns>
    public static IServiceCollection AddTelegramBot(this IServiceCollection services,
                                                    Action<BotSettingsBuilder<TelegramBotSettings>> optionsBuilderFunc,
                                                    Action<AnalyticsClientSettingsBuilder<AnalyticsClientSettings>> analyticsOptionsBuilderFunc,
                                                    Action<ServerSettingsBuilder<ServerSettings>> serverSettingsBuilderFunc,
                                                    Action<DataAccessSettingsBuilder<DataAccessSettings>> dataAccessSettingsBuilderFunc)
    {
        optionsBuilderFunc(SettingsBuilder);
        serverSettingsBuilderFunc(ServerSettingsBuilder);
        analyticsOptionsBuilderFunc(AnalyticsClientOptionsBuilder);
        dataAccessSettingsBuilderFunc(DataAccessSettingsBuilder);
        
        var clientBuilder = TelegramClientDecoratorBuilder.Instance(services, SettingsBuilder);
        
        var botBuilder = TelegramBotBuilder.Instance(services,
                                                     ServerSettingsBuilder,
                                                     SettingsBuilder, 
                                                     DataAccessSettingsBuilder,
                                                     AnalyticsClientOptionsBuilder)
                                           .AddClient(clientBuilder);
        var bot = botBuilder.Build();
        return services.AddSingleton<IBot<TelegramBot>>(bot)
                       .AddSingleton<IBot>(bot)
                       .AddTelegramLayoutsSupport();
    }

    public static IServiceCollection AddTelegramLayoutsSupport(this IServiceCollection services) =>
            services.AddScoped<ILayoutParser, JsonLayoutParser>()
                    .AddScoped<ILayoutSupplier<ReplyKeyboardMarkup>, ReplyTelegramLayoutSupplier>()
                    .AddScoped<ILayoutSupplier<InlineKeyboardMarkup>, InlineTelegramLayoutSupplier>()
                    .AddScoped<ILayoutLoader<ReplyKeyboardMarkup>, LayoutLoader<ILayoutParser, ILayoutSupplier<ReplyKeyboardMarkup>, ReplyKeyboardMarkup>>()
                    .AddScoped<ILayoutLoader<InlineKeyboardMarkup>, LayoutLoader<ILayoutParser, ILayoutSupplier<InlineKeyboardMarkup>, InlineKeyboardMarkup>>();
}