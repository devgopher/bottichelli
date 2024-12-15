using Botticelli.AI.AIProvider;
using Botticelli.AI.ChatGpt.Provider;
using Botticelli.AI.ChatGpt.Settings;
using Botticelli.AI.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Botticelli.AI.ChatGpt.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds a provider for ChatGpt-based solution
    /// </summary>
    /// <param name="services"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    /// <exception cref="AiException"></exception>
    public static IServiceCollection AddChatGptProvider(this IServiceCollection services, IConfiguration config)
    {
        var chatGptSettings = new GptSettings();
        config.Bind(nameof(GptSettings), chatGptSettings);

        services.Configure<GptSettings>(s =>
        {
            s.Model = chatGptSettings.Model;
            s.Temperature = chatGptSettings.Temperature;
            s.ApiKey = chatGptSettings.ApiKey;
            s.Url = chatGptSettings.Url;
            s.AiName = chatGptSettings.AiName;
            s.StreamGeneration = chatGptSettings.StreamGeneration;
        });

        services.AddSingleton<IAiProvider, ChatGptProvider>();

        return services;
    }
}