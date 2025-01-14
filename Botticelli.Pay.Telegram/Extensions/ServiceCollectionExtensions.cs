using Botticelli.Bot.Data.Settings;
using Botticelli.Client.Analytics.Settings;
using Botticelli.Framework.Options;
using Botticelli.Framework.Telegram.Extensions;
using Botticelli.Framework.Telegram.Options;
using Botticelli.Pay.Telegram.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace Botticelli.Pay.Telegram.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds a Telegram bot with a payment function
    /// </summary>
    /// <param name="services"></param>
    /// <param name="optionsBuilderFunc"></param>
    /// <param name="analyticsOptionsBuilderFunc"></param>
    /// <param name="serverSettingsBuilderFunc"></param>
    /// <param name="dataAccessSettingsBuilderFunc"></param>
    /// <returns></returns>
    public static IServiceCollection AddTelegramPayBot(this IServiceCollection services,
        Action<BotSettingsBuilder<TelegramBotSettings>> optionsBuilderFunc,
        Action<AnalyticsClientSettingsBuilder<AnalyticsClientSettings>> analyticsOptionsBuilderFunc,
        Action<ServerSettingsBuilder<ServerSettings>> serverSettingsBuilderFunc,
        Action<DataAccessSettingsBuilder<DataAccessSettings>> dataAccessSettingsBuilderFunc) => services.AddTelegramBot(
        optionsBuilderFunc,
        analyticsOptionsBuilderFunc,
        serverSettingsBuilderFunc,
        dataAccessSettingsBuilderFunc,
        o => o.AddHandler<BotPreCheckoutHandler>());
}