using System.Net.Http.Headers;
using Botticelli.AI.Message;
using Botticelli.AI.Settings;
using Botticelli.Bot.Interfaces.Client;
using Botticelli.Shared.API.Client.Responses;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Botticelli.AI.AIProvider;

public abstract class ChatGptProvider<TSettings> : IAiProvider
    where TSettings : AiSettings
{
    private readonly IHttpClientFactory? _factory;
    private readonly IValidator<AiMessage>? _messageValidator;
    protected readonly IBusClient? Bus;
    protected readonly ILogger Logger;
    protected readonly IOptions<TSettings> Settings;

    protected ChatGptProvider(IOptions<TSettings> settings,
        IHttpClientFactory? factory,
        ILogger logger,
        IBusClient? bus,
        IValidator<AiMessage>? messageValidator)
    {
        Settings = settings;
        _factory = factory;
        Logger = logger;
        Bus = bus;
        _messageValidator = messageValidator;
    }

    public virtual async Task SendAsync(AiMessage message, CancellationToken token)
    {
        await ValidateMessage(message, token);

        try
        {
            Logger.LogDebug($"{nameof(SendAsync)}({message.ChatIds}) started");

            using var client = GetClient();

            var response = await GetGptResponse(message, token, client);

            if (response.IsSuccessStatusCode)
            {
                await ProcessGptResponse(message, token, response);
            }
            else
            {
                var reason = await response.Content.ReadAsStringAsync(token);
                await SendErrorGptResponse(message, reason, token);
            }

            Logger.LogDebug($"{nameof(SendAsync)}({message.ChatIds}) finished");
        }
        catch (Exception ex)
        {
            await SendErrorResponse(message, token, ex);
        }
    }

    public abstract string AiName { get; }

    private async Task SendErrorResponse(AiMessage message, CancellationToken token, Exception ex)
    {
        Logger.LogError(ex, ex.Message);
        await Bus.SendResponse(new SendMessageResponse(message.Uid)
            {
                IsPartial = false,
                Message = new Shared.ValueObjects.Message(message.Uid)
                {
                    ChatIds = message.ChatIds,
                    Subject = message.Subject,
                    Body = $"Error getting a response from {AiName}!",
                    Attachments = null,
                    From = null,
                    ForwardedFrom = null
                }
            },
            token);
    }

    protected async Task ValidateMessage(AiMessage message, CancellationToken token)
    {
        Logger.LogError($"{nameof(SendAsync)}() body is null or empty!");

        await _messageValidator.ValidateAsync(message, token);
    }

    private HttpClient GetClient()
    {
        var client = _factory.CreateClient();

        client.BaseAddress = new Uri(Settings.Value.Url);
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", Settings.Value.ApiKey);

        return client;
    }

    protected async Task SendErrorGptResponse(AiMessage message, string reason, CancellationToken token)
    {
        await Bus.SendResponse(new SendMessageResponse(message.Uid)
            {
                Message = new Shared.ValueObjects.Message(message.Uid)
                {
                    ChatIds = message.ChatIds,
                    Subject = message.Subject,
                    Body = $"Error getting a response from {AiName}: {reason}!",
                    Attachments = null,
                    From = null,
                    ForwardedFrom = null,
                    ReplyToMessageUid = message.ReplyToMessageUid
                }
            },
            token);
    }

    protected abstract Task ProcessGptResponse(AiMessage message, CancellationToken token,
        HttpResponseMessage response);

    protected abstract Task<HttpResponseMessage> GetGptResponse(AiMessage message, CancellationToken token,
        HttpClient client);
}