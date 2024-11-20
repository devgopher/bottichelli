using Botticelli.Audio;
using Botticelli.Framework.Options;
using Botticelli.Framework.Vk.Messages.Options;
using Microsoft.Extensions.Logging;

namespace Botticelli.Framework.Vk.Messages.Builders;

/// <summary>
/// Builder for VK media uploader
/// </summary>
public class VkStorageUploaderBuilder
{
    private IHttpClientFactory _httpClientFactory;
    private ILogger<MessagePublisher> _logger;
    private IConvertor _audioConvertor;

    public static VkStorageUploaderBuilder Instance(BotSettingsBuilder<VkBotSettings> settingsBuilder) 
        => new();

    private VkStorageUploaderBuilder() {}

    public VkStorageUploaderBuilder AddLogger(ILogger<MessagePublisher> logger)
    {
        _logger = logger;

        return this;
    }

    public VkStorageUploaderBuilder AddHttpClientFactory(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;

        return this;
    }
    
    public VkStorageUploaderBuilder AddAudioConvertor(IConvertor audioConvertor)
    {
        _audioConvertor = audioConvertor;

        return this;
    }
    
    public VkStorageUploader? Build() => new(_httpClientFactory, _audioConvertor, _logger);
}