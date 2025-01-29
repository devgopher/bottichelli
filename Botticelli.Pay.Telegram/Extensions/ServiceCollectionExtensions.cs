using Botticelli.Bot.Data.Settings;
using Botticelli.Client.Analytics.Settings;
using Botticelli.Framework.Options;
using Botticelli.Framework.Telegram.Extensions;
using Botticelli.Framework.Telegram.Options;
using Botticelli.Pay.Extensions;
using Botticelli.Pay.Handlers;
using Botticelli.Pay.Models;
using Botticelli.Pay.Processors;
using Botticelli.Pay.Telegram.Handlers;
using Microsoft.Extensions.Configuration;
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
    public static IServiceCollection AddTelegramPayBot<THandler, TProcessor, TQuery>(this IServiceCollection services,
        Action<BotSettingsBuilder<TelegramBotSettings>> optionsBuilderFunc,
        Action<AnalyticsClientSettingsBuilder<AnalyticsClientSettings>> analyticsOptionsBuilderFunc,
        Action<ServerSettingsBuilder<ServerSettings>> serverSettingsBuilderFunc,
        Action<DataAccessSettingsBuilder<DataAccessSettings>> dataAccessSettingsBuilderFunc)
        where THandler : IPreCheckoutHandler, new()
        where TProcessor : IPayProcessor<THandler, TQuery>
    {
        services.AddPayments<THandler, TProcessor, TQuery>();

        return services.AddTelegramBot<TelegramPaymentBot>(
            optionsBuilderFunc,
            analyticsOptionsBuilderFunc,
            serverSettingsBuilderFunc,
            dataAccessSettingsBuilderFunc,
            o => o.AddSubHandler<BotPreCheckoutSubHandler>()
                .AddSubHandler<BotSuccessfulPaymentSubHandler>());
    }

    /// <summary>
    ///     Adds a Telegram bot with a payment function
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddTelegramPayBot<THandler, TProcessor>(this IServiceCollection services, IConfiguration configuration)
        where THandler : IPreCheckoutHandler, new()
        where TProcessor : IPayProcessor<THandler, PreCheckoutQuery>
    {
        services.AddPayments<THandler, TProcessor, PreCheckoutQuery>();

        return services.AddTelegramBot<TelegramPaymentBot>(configuration, o => o.AddSubHandler<BotPreCheckoutSubHandler>()
            .AddSubHandler<BotSuccessfulPaymentSubHandler>());
    }
}