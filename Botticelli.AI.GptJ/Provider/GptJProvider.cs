using System.Net.Http.Headers;
using System.Net.Http.Json;
using Botticelli.AI.AIProvider;
using Botticelli.AI.GptJ.Message.GptJ;
using Botticelli.AI.GptJ.Settings;
using Botticelli.AI.Message;
using Botticelli.Bot.Interfaces.Client;
using Botticelli.Shared.API.Client.Responses;
using FluentValidation;
using Flurl;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Botticelli.AI.GptJ.Provider;

public class GptJProvider : ChatGptProvider<AiGptSettings>
{
    public GptJProvider(IOptionsSnapshot<AiGptSettings> gptSettings,
        IHttpClientFactory? factory,
        ILogger<GptJProvider> logger,
        IBusClient? bus,
        IValidator<AiMessage>? messageValidator) : base(gptSettings,
        factory,
        logger,
        bus,
        messageValidator)
    {
    }

    public override string AiName => "gptj";

    protected override async Task ProcessGptResponse(AiMessage message, CancellationToken token,
        HttpResponseMessage response)
    {
        var outMessage = await response.Content.ReadFromJsonAsync<GptJOutputMessage>(token);

        await Bus.SendResponse(new SendMessageResponse(message.Uid)
            {
                IsPartial = false,
                Message = new Shared.ValueObjects.Message(message.Uid)
                {
                    ChatIds = message.ChatIds,
                    Subject = message.Subject,
                    Body = outMessage?.Completion,
                    Attachments = null,
                    From = null,
                    ForwardedFrom = null,
                    ReplyToMessageUid = message.ReplyToMessageUid
                }
            },
            token);
    }

    protected override async Task<HttpResponseMessage> GetGptResponse(AiMessage message, CancellationToken token,
        HttpClient client)
    {
        client.BaseAddress = new Uri(Settings.Value.Url);

        if (!string.IsNullOrWhiteSpace(Settings.Value.ApiKey))
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", Settings.Value.ApiKey);

        var content = JsonContent.Create(new GptJInputMessage
        {
            Text = message.Body,
            GenerateTokensLimit = Settings.Value.GenerateTokensLimit,
            TopP = Settings.Value.TopP,
            TopK = Settings.Value.TopK,
            Temperature = Settings.Value.Temperature
        });

        Logger.LogDebug($"{nameof(SendAsync)}({message.ChatIds}) content: {content.Value}");

        return await client.PostAsync(Url.Combine($"{Settings.Value.Url}", "generate"),
            content,
            token);
    }
}