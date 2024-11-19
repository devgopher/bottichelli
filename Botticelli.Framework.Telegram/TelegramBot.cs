using System.Text;
using Botticelli.Bot.Data.Entities.Bot;
using Botticelli.Bot.Data.Repositories;
using Botticelli.Bot.Utils.TextUtils;
using Botticelli.Client.Analytics;
using Botticelli.Framework.Events;
using Botticelli.Framework.Exceptions;
using Botticelli.Framework.Global;
using Botticelli.Framework.Telegram.Handlers;
using Botticelli.Interfaces;
using Botticelli.Shared.API;
using Botticelli.Shared.API.Admin.Requests;
using Botticelli.Shared.API.Admin.Responses;
using Botticelli.Shared.API.Client.Requests;
using Botticelli.Shared.API.Client.Responses;
using Botticelli.Shared.Constants;
using Botticelli.Shared.Utils;
using Botticelli.Shared.ValueObjects;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Exception = System.Exception;
using Message = Telegram.Bot.Types.Message;
using Poll = Botticelli.Shared.ValueObjects.Poll;

namespace Botticelli.Framework.Telegram;

public class TelegramBot : BaseBot<TelegramBot>
{
    private readonly IBotDataAccess _data;
    private readonly IBotUpdateHandler _handler;
    private readonly ITextTransformer _textTransformer;
    private ITelegramBotClient _client;

    public TelegramBot(ITelegramBotClient client,
        IBotUpdateHandler handler,
        ILogger<TelegramBot> logger,
        MetricsProcessor metrics,
        ITextTransformer textTransformer,
        IBotDataAccess data) : base(logger, metrics)
    {
        BotStatusKeeper.IsStarted = false;
        _client = client;
        _handler = handler;
        _textTransformer = textTransformer;
        _data = data;
        BotUserId = _client.BotId.ToString();
    }

    public override BotType Type => BotType.Telegram;
    public override event MsgSentEventHandler MessageSent;
    public override event MsgReceivedEventHandler MessageReceived;
    public override event MsgRemovedEventHandler MessageRemoved;
    public override event MessengerSpecificEventHandler MessengerSpecificEvent;

    /// <summary>
    ///     Deletes a message
    /// </summary>
    /// <param name="request"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="BotException"></exception>
    protected override async Task<RemoveMessageResponse> InnerDeleteMessageAsync(RemoveMessageRequest request,
        CancellationToken token)
    {
        if (!BotStatusKeeper.IsStarted)
        {
            Logger.LogInformation("Bot wasn't started!");

            return new RemoveMessageResponse(request.Uid, "Bot wasn't started!")
            {
                MessageRemovedStatus = MessageRemovedStatus.NotStarted
            };
        }

        RemoveMessageResponse response = new(request.Uid, string.Empty);

        try
        {
            if (string.IsNullOrWhiteSpace(request.Uid)) throw new BotException("request/message is null!");

            await _client.DeleteMessageAsync(request.ChatId,
                int.Parse(request.Uid),
                token);
            response.MessageRemovedStatus = MessageRemovedStatus.Ok;
        }
        catch
        {
            response.MessageRemovedStatus = MessageRemovedStatus.Fail;
        }

        response.MessageUid = request.Uid;

        var eventArgs = new MessageRemovedBotEventArgs
        {
            MessageUid = request.Uid
        };

        MessageRemoved?.Invoke(this, eventArgs);

        return response;
    }

    /// <summary>
    ///     Sends a message as a telegram bot
    /// </summary>
    /// <param name="request"></param>
    /// <param name="optionsBuilder"></param>
    /// <param name="isUpdate"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="BotException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    protected override async Task<SendMessageResponse> InnerSendMessageAsync<TSendOptions>(SendMessageRequest request,
        ISendOptionsBuilder<TSendOptions> optionsBuilder,
        bool isUpdate,
        CancellationToken token)
    {
        if (!BotStatusKeeper.IsStarted)
        {
            Logger.LogInformation("Bot wasn't started!");

            return new SendMessageResponse(request.Uid, "Bot wasn't started!")
            {
                MessageSentStatus = MessageSentStatus.Nonstarted
            };
        }

        SendMessageResponse response = new(request.Uid, string.Empty);

        IReplyMarkup replyMarkup;

        if (optionsBuilder == default)
            replyMarkup = null;
        else if (optionsBuilder.Build() is IReplyMarkup)
            replyMarkup = optionsBuilder.Build() as IReplyMarkup;
        else
            replyMarkup = null;

        try
        {
            if (request?.Message == default) throw new BotException("request/message is null!");

            var text = new StringBuilder($"{request.Message.Subject} {request.Message.Body}");
            var retText = _textTransformer.Escape(text).ToString();
            List<(string chatId, string innerId)> pairs = [];

            foreach (var link in request.Message.ChatIdInnerIdLinks)
                pairs.AddRange(link.Value.Select(innerId => (link.Key, innerId)));

            var chatIdOnly = request.Message.ChatIds.Where(c => !request.Message.ChatIdInnerIdLinks.ContainsKey(c));
            pairs.AddRange(chatIdOnly.Select(c => (c, string.Empty)));

            Logger.LogInformation($"Pairs count: {pairs.Count}");

            for (var i = 0; i < pairs.Count; i++)
            {
                var link = pairs[i];
                Message message = null;

                if (!string.IsNullOrWhiteSpace(retText))
                {
                    if (!(request.ExpectPartialResponse ?? false))
                    {
                        Logger.LogInformation("No streaming response - sending a message!");
                        await SendText(retText);
                    }
                    else
                    {
                        Logger.LogWarning(@"Streaming output isn't supported for Telegram now!");
                        await SendText(@"Sorry, but streaming output isn't supported for Telegram now!");
                    }

                    async Task SendText(string sendText)
                    {
                        if (!isUpdate)
                        {
                            var sentMessage = await _client.SendMessage(link.chatId,
                                sendText,
                                ParseMode.MarkdownV2,
                                GetReplyParameters(request, link.chatId),
                                replyMarkup,
                                cancellationToken: token);

                            link.innerId = sentMessage.MessageId.ToString();
                        }
                        else
                        {
                            await _client.EditMessageText(link.chatId,
                                int.Parse(link.innerId),
                                sendText,
                                ParseMode.MarkdownV2,
                                replyMarkup: replyMarkup as InlineKeyboardMarkup,
                                cancellationToken: token);
                        }
                    }
                }

                if (request.Message?.Poll != default)
                {
                    var type = request.Message.Poll?.Type switch
                    {
                        Poll.PollType.Quiz => PollType.Quiz,
                        Poll.PollType.Regular => PollType.Regular,
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    message = await _client.SendPoll(link.chatId,
                        request.Message.Poll?.Question ?? "No question",
                        GetPollOptions(request),
                        request.Message.Poll?.IsAnonymous ?? false,
                        type,
                        correctOptionId: request.Message.Poll?.CorrectAnswerId,
                        replyParameters: GetReplyParameters(request, link.chatId),
                        replyMarkup: replyMarkup,
                        cancellationToken: token);
                    AddChatIdInnerIdLink(response, link.chatId, message);
                }

                if (request.Message?.Contact != default)
                    await SendContact(request,
                        response,
                        token,
                        replyMarkup);

                if (request.Message?.Attachments == null) continue;

                foreach (var attachment in request.Message
                             .Attachments
                             .Where(a => a is BinaryBaseAttachment)
                             .Cast<BinaryBaseAttachment>())
                    switch (attachment.MediaType)
                    {
                        case MediaType.Audio:
                            var audio = new InputFileStream(attachment.Data.ToStream(), attachment.Name);
                            message = await _client.SendAudio(link.chatId,
                                audio,
                                request.Message.Subject,
                                ParseMode.MarkdownV2, GetReplyParameters(request, link.chatId),
                                replyMarkup,
                                cancellationToken: token);
                            AddChatIdInnerIdLink(response, link.chatId, message);

                            break;
                        case MediaType.Video:
                            var video = new InputFileStream(attachment.Data.ToStream(), attachment.Name);
                            message = await _client.SendVideo(link.chatId,
                                video,
                                replyParameters: GetReplyParameters(request, link.chatId),
                                replyMarkup: replyMarkup,
                                cancellationToken: token);
                            AddChatIdInnerIdLink(response, link.chatId, message);

                            break;
                        case MediaType.Image:
                            var image = new InputFileStream(attachment.Data.ToStream(), attachment.Name);
                            message = await _client.SendPhoto(link.chatId,
                                image,
                                replyParameters: GetReplyParameters(request, link.chatId),
                                replyMarkup: replyMarkup,
                                cancellationToken: token);
                            AddChatIdInnerIdLink(response, link.chatId, message);

                            break;
                        case MediaType.Voice:
                            var voice = new InputFileStream(attachment.Data.ToStream(), attachment.Name);
                            message = await _client.SendVoice(link.chatId,
                                voice,
                                request.Message.Subject,
                                ParseMode.MarkdownV2,
                                GetReplyParameters(request, link.chatId),
                                replyMarkup,
                                cancellationToken: token);
                            AddChatIdInnerIdLink(response, link.chatId, message);

                            break;
                        case MediaType.Sticker:
                            InputFile sticker = string.IsNullOrWhiteSpace(attachment.Url)
                                ? new InputFileStream(attachment.Data.ToStream(), attachment.Name)
                                : new InputFileUrl(attachment.Url);

                            message = await _client.SendSticker(link.chatId,
                                sticker,
                                GetReplyParameters(request, link.chatId),
                                replyMarkup,
                                cancellationToken: token);
                            AddChatIdInnerIdLink(response, link.chatId, message);

                            break;
                        case MediaType.Contact:
                            await SendContact(request,
                                response,
                                token,
                                replyMarkup);

                            break;
                        case MediaType.Document:
                            var doc = new InputFileStream(attachment.Data.ToStream(), attachment.Name);
                            message = await _client.SendDocument(link.chatId,
                                doc,
                                replyParameters: GetReplyParameters(request, link.chatId),
                                replyMarkup: replyMarkup,
                                cancellationToken: token);
                            AddChatIdInnerIdLink(response, link.chatId, message);

                            break;
                        case MediaType.Unknown:
                        case MediaType.Poll:
                        case MediaType.Text:
                        default:
                            // nothing to do
                            break;
                    }

                AddChatIdInnerIdLink(response, link.chatId, message);
            }

            response.MessageSentStatus = MessageSentStatus.Ok;

            var eventArgs = new MessageSentBotEventArgs
            {
                Message = request.Message
            };

            MessageSent?.Invoke(this, eventArgs);
        }
        catch (Exception ex)
        {
            response.MessageSentStatus = MessageSentStatus.Fail;
            Logger.LogError(ex, ex.Message);
        }

        return response;
    }

    private static InputPollOption[] GetPollOptions(SendMessageRequest request)
    {
        return request.Message.Poll?.Variants?.Select(po =>
            new InputPollOption
            {
                Text = po
            })?.ToArray() ?? [];
    }

    private static void AddChatIdInnerIdLink(SendMessageResponse response, string chatId, Message message)
    {
        message.NotNull();
        if (!response.Message.ChatIdInnerIdLinks.ContainsKey(chatId))
            response.Message.ChatIdInnerIdLinks[chatId] = [];

        response.Message.ChatIdInnerIdLinks[chatId].Add(message!.MessageId.ToString());
    }

    private async Task SendContact(SendMessageRequest request,
        SendMessageResponse response,
        CancellationToken token,
        IReplyMarkup replyMarkup)
    {
        foreach (var chatId in request.Message?.ChatIds)
            try
            {
                var message = await _client.SendContact(chatId,
                    request.Message.Contact?.Phone,
                    request.Message?.Contact?.Name,
                    request.Message?.Contact?.Surname,
                    replyParameters: GetReplyParameters(request, chatId),
                    replyMarkup: replyMarkup,
                    cancellationToken: token);

                AddChatIdInnerIdLink(response, chatId, message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
            }
    }

    private static ReplyParameters GetReplyParameters(SendMessageRequest request, string chatId) =>
        new()
        {
            ChatId = chatId,
            MessageId = request.Message.ReplyToMessageUid != default
                ? int.Parse(request.Message.ReplyToMessageUid)
                : default
        };

    /// <summary>
    ///     Starts a bot
    /// </summary>
    /// <param name="request"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected override Task<StartBotResponse> InnerStartBotAsync(StartBotRequest request, CancellationToken token)
    {
        try
        {
            Logger.LogInformation($"{nameof(StartBotAsync)}...");
            var response = StartBotResponse.GetInstance(AdminCommandStatus.Ok, "");

            if (BotStatusKeeper.IsStarted)
            {
                Logger.LogInformation($"{nameof(StartBotAsync)}: already started");

                return Task.FromResult(response);
            }

            BotStatusKeeper.IsStarted = true;

            // Rethrowing an event from BotUpdateHandler
            _handler.MessageReceived += (sender, e)
                =>
            {
                MessageReceived?.Invoke(sender, e);
            };

            _client.StartReceiving(_handler, cancellationToken: token);

            Logger.LogInformation($"{nameof(StartBotAsync)}: started");

            return Task.FromResult(response);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
        }

        return Task.FromResult(StartBotResponse.GetInstance(AdminCommandStatus.Fail, "error"));
    }

    /// <summary>
    ///     Stops a bot
    /// </summary>
    /// <param name="request"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected override async Task<StopBotResponse> InnerStopBotAsync(StopBotRequest request, CancellationToken token)
    {
        try
        {
            Logger.LogInformation($"{nameof(InnerStopBotAsync)}...");
            var response = StopBotResponse.GetInstance(request.Uid, "", AdminCommandStatus.Ok);

            if (!BotStatusKeeper.IsStarted) return response;

            BotStatusKeeper.IsStarted = false;

            await _client.Close(token);

            Logger.LogInformation($"{nameof(StopBotAsync)}: stopped");

            return response;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
        }

        return StopBotResponse.GetInstance(AdminCommandStatus.Fail, "error");
    }

    private void RecreateClient(string key) => _client = new TelegramBotClient(key);

    private async Task StartBot(CancellationToken token)
    {
        var startRequest = StartBotRequest.GetInstance();
        await StartBotAsync(startRequest, token);
    }

    private async Task StopBot(CancellationToken token)
    {
        var stopRequest = StopBotRequest.GetInstance();
        await StopBotAsync(stopRequest, token);
    }

    public override async Task SetBotContext(BotData? context, CancellationToken token)
    {
        if (context is null) return;

        var currentContext = _data.GetData();

        if (currentContext?.BotKey != context.BotKey)
        {
            await StopBot(token);

            _data.SetData(context);
            RecreateClient(context.BotKey!);

            await StartBot(token);
        }
        else if (currentContext != null)
        {
            if (_client.BotId == default)
            {
                await StopBot(token);
                RecreateClient(context.BotKey!);
                await StartBot(token);
            }
        }
    }
}