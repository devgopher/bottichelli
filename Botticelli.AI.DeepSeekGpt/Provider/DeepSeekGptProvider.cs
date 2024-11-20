using System.Net.Http.Headers;
using System.Net.Http.Json;
using Botticelli.AI.AIProvider;
using Botticelli.AI.DeepSeekGpt.Message.DeepSeek;
using Botticelli.AI.DeepSeekGpt.Settings;
using Botticelli.AI.Exceptions;
using Botticelli.AI.Message;
using Botticelli.Bot.Interfaces.Client;
using Botticelli.Shared.API.Client.Responses;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Botticelli.AI.DeepSeekGpt.Provider;

public class DeepSeekGptProvider : ChatGptProvider<DeepSeekGptSettings>
{
    private const string SystemRole = "system";
    private const string UserRole = "user";
    private const string Completion = "completions";

    public DeepSeekGptProvider(IOptions<DeepSeekGptSettings> gptSettings,
        IHttpClientFactory? factory,
        ILogger<DeepSeekGptProvider> logger,
        IBusClient? bus,
        IValidator<AiMessage>? messageValidator) : base(gptSettings,
        factory,
        logger,
        bus,
        messageValidator)
    {
    }


    protected override async Task ProcessGptResponse(AiMessage message, CancellationToken token,
        HttpResponseMessage response)
    {
        var outMessage =
            await response.Content.ReadFromJsonAsync<DeepSeekOutputMessage>(cancellationToken: token);

        if (outMessage == null)
            throw new AiException($"{nameof(outMessage)} = null!");

        await Bus.SendResponse(new SendMessageResponse(message.Uid)
            {
                Message = new Shared.ValueObjects.Message(message.Uid)
                {
                    ChatIds = message.ChatIds,
                    Subject = message.Subject,
                    Body = string.Join(" ",
                        outMessage.Choices.Select(c => c.DeepSeekMessage?.Content ?? string.Empty)),
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
        var deepSeekGptMessage = new DeepSeekInputMessage
        {
            Model = Settings.Value.Model,
            MaxTokens = Settings.Value.MaxTokens,
            Messages = new List<DeepSeekInnerInputMessage>
            {
                new()
                {
                    Role = SystemRole,
                    Content = Settings.Value.Instruction
                },
                new()
                {
                    Role = UserRole,
                    Content = message.Body
                }
            }
        };

        deepSeekGptMessage.Messages.AddRange(message.AdditionalMessages?.Select(m => new DeepSeekInnerInputMessage
                                             {
                                                 Role = UserRole,
                                                 Content = m.Body ?? string.Empty
                                             }) ??
                                             new List<DeepSeekInnerInputMessage>());

        var content = JsonContent.Create(deepSeekGptMessage);

        Logger.LogDebug($"{nameof(SendAsync)}({message.ChatIds}) content: {content.Value}");

        return await client.PostAsync(Completion,
            content,
            token);
    }

    public override string AiName => "deepseek";
}
