using Botticelli.Framework.Options;
using Botticelli.Framework.Vk.Messages.Options;
using Microsoft.Extensions.Logging;

namespace Botticelli.Framework.Vk.Messages.Builders;

/// <summary>
/// Builds a Long poll message provider|receiver
/// </summary>
public class LongPollMessagesProviderBuilder
{
    private IHttpClientFactory _httpClientFactory;
    private ILogger<LongPollMessagesProvider> _logger;
    private readonly BotSettingsBuilder<VkBotSettings> _settingsBuilder;

    public static LongPollMessagesProviderBuilder Instance(BotSettingsBuilder<VkBotSettings> settingsBuilder) 
        => new(settingsBuilder);

    private LongPollMessagesProviderBuilder(BotSettingsBuilder<VkBotSettings> settingsBuilder) => _settingsBuilder = settingsBuilder;

    public LongPollMessagesProviderBuilder AddLogger(ILogger<LongPollMessagesProvider> logger)
    {
        _logger = logger;

        return this;
    }

    public LongPollMessagesProviderBuilder AddHttpClientFactory(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;

        return this;
    }
    
    public LongPollMessagesProvider Build() => new(_settingsBuilder.Build(), _httpClientFactory, _logger);
}