using System.Net.Http.Json;
using System.Text;
using Botticelli.AI.AIProvider;
using Botticelli.AI.ChatGpt.Message.ChatGpt;
using Botticelli.AI.ChatGpt.Settings;
using Botticelli.AI.Message;
using Botticelli.Bot.Interfaces.Client;
using Botticelli.Shared.API.Client.Responses;
using FluentValidation;
using Flurl;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Botticelli.AI.ChatGpt.Provider;

public class ChatGptProvider : ChatGptProvider<GptSettings>
{
    public ChatGptProvider(IOptions<GptSettings> gptSettings,
        IHttpClientFactory? factory,
        ILogger<ChatGptProvider> logger,
        IBusClient? bus,
        IValidator<AiMessage>? messageValidator) : base(gptSettings,
        factory,
        logger,
        bus,
        messageValidator)
    {
    }

    public override string AiName => "chatgpt";

    protected override async Task ProcessGptResponse(AiMessage message, CancellationToken token,
        HttpResponseMessage response)
    {
        var text = new StringBuilder();

        var outStream = await response.Content.ReadAsStreamAsync(token);
        using var sr = new StreamReader(outStream);

        using var reader = TextReader.Synchronized(sr);
        var partText = Settings.Value.StreamGeneration
            ? await reader.ReadLineAsync(token)
            : await reader.ReadToEndAsync(token);
        var seqNumber = 0;
        while (partText != null)
        {
            try
            {
                if (Settings.Value.StreamGeneration)
                    partText = partText.Replace("data: ", string.Empty);

                var part = JsonConvert.DeserializeObject<ChatGptOutputMessage>(partText);

                text.AppendJoin(' ',
                    part?.Choices?
                        .Select(c => (c.Message ?? c.Delta)?.Content) ?? Array.Empty<string>());

                var responseMessage = new SendMessageResponse(message.Uid)
                {
                    IsPartial = Settings.Value.StreamGeneration,
                    SequenceNumber = seqNumber++,
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
                };

                await Bus.SendResponse(responseMessage,
                    token);

                if (Settings.Value.StreamGeneration)
                    if (part?.Choices?.Any(
                            c => c.FinishReason != null ? c.FinishReason.Contains("stop") : false) ==
                        true)
                        break;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
            }

            if (Settings.Value.StreamGeneration)
                partText = await reader.ReadLineAsync(token);
            else
                break;
        }
    }

    protected override async Task<HttpResponseMessage> GetGptResponse(AiMessage message, CancellationToken token,
        HttpClient client)
    {
        var content = JsonContent.Create(new ChatGptInputMessage
        {
            Model = Settings.Value.Model,
            Messages = new List<ChatGptMessage>
            {
                new()
                {
                    Role = "user",
                    Content = message.Body
                }
            },
            Temperature = Settings.Value.Temperature,
            Stream = Settings.Value.StreamGeneration
        });

        var response = await client.PostAsync(Url.Combine($"{Settings.Value.Url}", "completions"),
            content,
            token);
        return response;
    }
}