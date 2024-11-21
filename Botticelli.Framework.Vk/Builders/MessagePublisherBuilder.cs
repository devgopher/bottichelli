using Botticelli.Framework.Options;
using Botticelli.Framework.Vk.Messages.Options;
using Microsoft.Extensions.Logging;

namespace Botticelli.Framework.Vk.Messages.Builders;

/// <summary>
/// Builds a MessagePublisher
/// </summary>
public class MessagePublisherBuilder
{
    private IHttpClientFactory _httpClientFactory;
    private ILogger<MessagePublisher> _logger;
    private readonly BotSettingsBuilder<VkBotSettings> _settingsBuilder;

    public static MessagePublisherBuilder Instance(BotSettingsBuilder<VkBotSettings> settingsBuilder) 
        => new(settingsBuilder);

    private MessagePublisherBuilder(BotSettingsBuilder<VkBotSettings> settingsBuilder) => _settingsBuilder = settingsBuilder;

    public MessagePublisherBuilder AddLogger(ILogger<MessagePublisher> logger)
    {
        _logger = logger;

        return this;
    }

    public MessagePublisherBuilder AddHttpClientFactory(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;

        return this;
    }
    
    public MessagePublisher? Build() => new(_httpClientFactory, _logger);
}