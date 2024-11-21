using System.Net.Http.Json;
using System.Text;
using Botticelli.AI.AIProvider;
using Botticelli.AI.Message;
using Botticelli.AI.YaGpt.Message.YaGpt;
using Botticelli.AI.YaGpt.Settings;
using Botticelli.Bot.Interfaces.Client;
using Botticelli.Shared.API.Client.Responses;
using FluentValidation;
using Flurl;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Botticelli.AI.YaGpt.Provider;

public class YaGptProvider : ChatGptProvider<YaGptSettings>
{
    private const string SystemRole = "system";
    private const string UserRole = "user";
    private const string Completion = "completion";

    public YaGptProvider(IOptions<YaGptSettings> gptSettings,
        IHttpClientFactory? factory,
        ILogger<YaGptProvider> logger,
        IBusClient? bus, 
        IValidator<AiMessage>? messageValidator) : base(gptSettings,
                                      factory,
                                      logger,
                                      bus,
                                      messageValidator)
    {
    }

    protected override async Task ProcessGptResponse(AiMessage message, CancellationToken token, HttpResponseMessage response)
    {
        var text = new StringBuilder();

        var outStream = await response.Content.ReadAsStreamAsync(token);
        var serializer = new JsonSerializer();
        using var sr = new StreamReader(outStream);
        await using var reader = new JsonTextReader(sr)
        {
            SupportMultipleContent = true
        };

        while (await reader.ReadAsync(token))
        {
            if (reader.TokenType != JsonToken.StartObject)
                continue;
                    
            var part = serializer.Deserialize<YaGptOutputMessage>(reader);

            text.AppendJoin(' ',
                part?.Result?
                    .Alternatives?
                    .Select(c => c.Message.Text) ?? Array.Empty<string>());

            await Bus.SendResponse(new SendMessageResponse(message.Uid)
                {
                    Message = new Shared.ValueObjects.Message(message.Uid)
                    {
                        ChatIds = message.ChatIds,
                        Subject = message.Subject,
                        Body = text.ToString(),
                        Attachments = null,
                        From = null,
                        ForwardedFrom = null,
                        ReplyToMessageUid = message.ReplyToMessageUid
                    }
                },
                token);

            if (Settings.Value.StreamGeneration)
                if (part?.Result?.Alternatives?.Any(c => c.Message.Text.Contains("ALTERNATIVE_STATUS_FINAL")) ==
                    true)
                    break;
        }
    }

    protected override async Task<HttpResponseMessage> GetGptResponse(AiMessage message, CancellationToken token,
        HttpClient client)
    {
        var yaGptMessage = new YaGptInputMessage
        {
            ModelUri = Settings.Value.Model,
            Messages = new List<YaGptMessage>
            {
                new()
                {
                    Role = SystemRole,
                    Text = Settings.Value.Instruction
                },
                new()
                {
                    Role = UserRole,
                    Text = message.Body
                }
            },
            CompletionOptions = new CompletionOptions()
            {
                MaxTokens = Settings.Value.MaxTokens,
                Stream = Settings.Value.StreamGeneration,
                Temperature = Settings.Value.Temperature
            }
        };

        yaGptMessage.Messages.AddRange(message.AdditionalMessages?.Select(m => new YaGptMessage()
        {
            Role = UserRole,
            Text = m.Body
        }) ?? new List<YaGptMessage>());

        var content = JsonContent.Create(yaGptMessage);

        Logger.LogDebug($"{nameof(SendAsync)}({message.ChatIds}) content: {content.Value}");

        return await client.PostAsync(Url.Combine($"{Settings.Value.Url}", Completion),
            content,
            token);
    }

    public override string AiName => "yagpt";
}