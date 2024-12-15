using Botticelli.Framework.Options;
using Botticelli.Framework.Telegram.Options;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace Botticelli.Framework.Telegram.Decorators;

public class TelegramClientDecoratorBuilder
{
    private readonly IServiceCollection _services;
    private readonly BotSettingsBuilder<TelegramBotSettings> _settingsBuilder;
    private HttpClient? _httpClient;
    private TelegramClientDecorator? _telegramClient;
    private IThrottler? _throttler;
    private string? _token;

    private TelegramClientDecoratorBuilder(IServiceCollection services,
        BotSettingsBuilder<TelegramBotSettings> settingsBuilder)
    {
        _services = services;
        _settingsBuilder = settingsBuilder;
    }

    public static TelegramClientDecoratorBuilder Instance(IServiceCollection services,
        BotSettingsBuilder<TelegramBotSettings> settingsBuilder)
        => new(services, settingsBuilder);

    public TelegramClientDecoratorBuilder AddThrottler(IThrottler throttler)
    {
        _throttler = throttler;

        return this;
    }

    public TelegramClientDecoratorBuilder AddToken(string? token)
    {
        _token = token;

        return this;
    }

    public TelegramClientDecorator Build()
    {
        _token ??= "11111111:AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";

        if (_telegramClient != null) return _telegramClient;

        if (_httpClient == default)
        {
            var factory = _services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>();

            _httpClient = factory.CreateClient(nameof(TelegramClientDecorator));
        }

        var botOptions = _settingsBuilder.Build();
        var clientOptions =
            new TelegramBotClientOptions(_token, botOptions.TelegramBaseUrl, botOptions.UseTestEnvironment ?? false);
        _telegramClient = new TelegramClientDecorator(clientOptions, _throttler, _httpClient);

        return _telegramClient;
    }
}