using System.Reflection;
using Botticelli.Framework.Commands.Validators;
using Botticelli.Framework.Controls.Parsers;
using Botticelli.Framework.Extensions;
using Botticelli.Framework.Telegram;
using Botticelli.Framework.Telegram.Layout;
using Botticelli.Locations.Commands;
using Botticelli.Locations.Commands.CommandProcessors;
using Botticelli.Locations.Integration;
using Botticelli.Locations.Options;
using Flurl;
using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nominatim.API.Address;
using Nominatim.API.Geocoders;
using Nominatim.API.Interfaces;
using Nominatim.API.Web;
using Telegram.Bot.Types.ReplyMarkups;

namespace Botticelli.Locations.Telegram.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds an OSM location provider
    /// </summary>
    /// <returns></returns>
    public static IServiceCollection AddOsmLocations(this IServiceCollection services,
        IConfiguration config,
        string url = "https://nominatim.openstreetmap.org")
    {
        services.AddHttpClient<OsmLocationProvider>();
        TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());
        return services.Configure<LocationsProcessorOptions>(config)
            .AddScoped<ICommandValidator<FindLocationsCommand>, PassValidator<FindLocationsCommand>>()
            .AddScoped<ICommandValidator<MapCommand>, PassValidator<MapCommand>>()
            .AddScoped<FindLocationsCommandProcessor<InlineKeyboardMarkup>>()
            .AddScoped<MapCommandProcessor<ReplyKeyboardMarkup>>()
            .AddScoped<ILocationProvider, OsmLocationProvider>()
            .AddScoped<INominatimWebInterface, NominatimWebInterface>()
            .AddScoped<IAddressSearcher, AddressSearcher>()
            .AddScoped<ILayoutSupplier<InlineKeyboardMarkup>, InlineTelegramLayoutSupplier>()
            .AddScoped<ILayoutSupplier<ReplyKeyboardMarkup>, ReplyTelegramLayoutSupplier>()
            .AddScoped<IForwardGeocoder, ForwardGeocoder>(sp =>
                new ForwardGeocoder(sp.GetRequiredService<INominatimWebInterface>(),
                    Url.Combine(url, "search")))
            .AddScoped<IReverseGeocoder, ReverseGeocoder>(sp =>
                new ReverseGeocoder(sp.GetRequiredService<INominatimWebInterface>(),
                    Url.Combine(url, "reverse")));
    }

    public static IServiceProvider RegisterOsmLocationsCommands(this IServiceProvider sp,
        string url = "https://nominatim.openstreetmap.org") =>
        sp.RegisterBotCommand<FindLocationsCommandProcessor<InlineKeyboardMarkup>, TelegramBot>()
          .RegisterBotCommand<MapCommandProcessor<ReplyKeyboardMarkup>, TelegramBot>();
}