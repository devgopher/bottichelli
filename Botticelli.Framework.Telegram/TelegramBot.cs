﻿using System.Text;
using BotDataSecureStorage;
using Botticelli.BotBase.Utils;
using Botticelli.Client.Analytics;
using Botticelli.Framework.Events;
using Botticelli.Framework.Exceptions;
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
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Message = Telegram.Bot.Types.Message;
using Poll = Botticelli.Shared.ValueObjects.Poll;

namespace Botticelli.Framework.Telegram;

// public class MessageSequenceState
// {
//     public SendMessageRequest Request { get; init; }
//     public int? InnerMessageId { get; set; }
//     public bool IsSent { get; set; }
// }
//
// public class MessageSequenceStates
// {
//     public MessageSequenceStates(string uid)
//     {
//         Uid = uid;
//         States = new List<MessageSequenceState>();
//     }
//
//     public string Uid { get; set; }
//     public List<MessageSequenceState> States { get; set; }
// }

public class TelegramBot : BaseBot<TelegramBot>
{
    private readonly IBotUpdateHandler _handler;
    private readonly SecureStorage _secureStorage;
    // private static readonly IMemoryCache Cache;
    private ITelegramBotClient _client;

    static TelegramBot()
    {
        // Cache = new MemoryCache(new MemoryDistributedCacheOptions
        // {
        //     ExpirationScanFrequency = TimeSpan.FromMinutes(1),
        //     SizeLimit = null
        // });
    }
    
    /// <summary>
    ///     ctor
    /// </summary>
    /// <param name="client"></param>
    /// <param name="handler"></param>
    /// <param name="logger"></param>
    /// <param name="metrics"></param>
    /// <param name="secureStorage"></param>
    public TelegramBot(ITelegramBotClient client,
        IBotUpdateHandler handler,
        ILogger<TelegramBot> logger,
        MetricsProcessor metrics,
        SecureStorage secureStorage) : base(logger, metrics)
    {
        IsStarted = false;
        _client = client;
        _handler = handler;
        _secureStorage = secureStorage;

        // Migration to a bot context instead of simple bot key
        _secureStorage.MigrateToBotContext(BotDataUtils.GetBotId());
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
        if (!IsStarted)
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
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="BotException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    protected override async Task<SendMessageResponse> InnerSendMessageAsync<TSendOptions>(SendMessageRequest request,
        ISendOptionsBuilder<TSendOptions> optionsBuilder,
        CancellationToken token)
    {
        if (!IsStarted)
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

            var retText = Escape(text).ToString();
            List<(string chatId, string innerId)> pairs = new();

            foreach (var link in request.Message.ChatIdInnerIdLinks)
                pairs.AddRange(link.Value.Select(innerId => (innerId, link.Key)));

            var chatIdOnly = request.Message.ChatIds.Where(c => !request.Message.ChatIdInnerIdLinks.ContainsKey(c));
            pairs.AddRange(chatIdOnly.Select(c => (c, string.Empty)));

            Logger.LogInformation($"Pairs count: {pairs.Count}");

            foreach (var link in pairs)
            {
                Message message = null;
                if (!string.IsNullOrWhiteSpace(retText))
                {
                    if (!(request.ExpectPartialResponse ?? false))
                    {
                        Logger.LogInformation($"No streaming response - sending a message!");
                        await _client.SendTextMessageAsync(link.chatId,
                            retText,
                            parseMode: ParseMode.MarkdownV2,
                            replyToMessageId: request.Message.ReplyToMessageUid != default
                                ? int.Parse(request.Message.ReplyToMessageUid)
                                : default,
                            replyMarkup: replyMarkup,
                            cancellationToken: token);
                    }
                    else
                    {
                        await _client.SendTextMessageAsync(link.chatId,
                            @"Sorry, but streaming output isn't supported for Telegram now\!",
                            parseMode: ParseMode.MarkdownV2,
                            replyToMessageId: request.Message.ReplyToMessageUid != default
                                ? int.Parse(request.Message.ReplyToMessageUid)
                                : default,
                            replyMarkup: replyMarkup,
                            cancellationToken: token);
                    }
                    // else
                    // {
                    //     Logger.LogInformation($"Streaming response - getting a message sequence!");
                    //     var messagesSequence = await Cache.GetOrCreateAsync(
                    //         request.Uid,
                    //         cacheEntry =>
                    //         {
                    //             cacheEntry.SlidingExpiration = TimeSpan.FromMinutes(30);
                    //             return Task.FromResult(new MessageSequenceStates(request.Uid));
                    //         });
                    //
                    //     Logger.LogInformation($"Message '{request.Uid}' sequence number: {request.SequenceNumber}");
                    //     if (!messagesSequence.States.Any())
                    //     {
                    //         message = await SendMessage<TSendOptions>(request, token, link, retText, replyMarkup, messagesSequence);
                    //     }
                    //     else
                    //     {
                    //         Logger.LogInformation(
                    //             $"Message '{request.Uid}' sequence number: {request.SequenceNumber}: trying to edit a first message");
                    //         var innerMessageId =
                    //             messagesSequence.States.FirstOrDefault(s => s.Request?.SequenceNumber == messagesSequence.States.Min(s => s.Request?.SequenceNumber))
                    //                 ?.InnerMessageId;
                    //         Logger.LogInformation($"Inner message '{request.Uid}' id {innerMessageId}");
                    //
                    //         if (innerMessageId == default)
                    //         {
                    //             Logger.LogInformation($"No inner message '{request.Uid}' id found in a cache!");
                    //             continue;
                    //         }
                    //
                    //
                    //         var lastCachedState = messagesSequence.States?.MaxBy(s => s.Request.SequenceNumber);
                    //         
                    //         if (request.SequenceNumber > lastCachedState.Request.SequenceNumber)
                    //         {
                    //             /*
                    //                 Since, there're some problems with editing a message in telegram (especially, messages with reply markup),
                    //                 editing is commented out - replacing messages
                    //              */
                    //             // Logger.LogInformation($"Trying to edit a message '{request.Uid}': {innerMessageId}");
                    //             // message = await _client.EditMessageTextAsync(link.chatId,
                    //             //     innerMessageId.Value!,
                    //             //     retText,
                    //             //     ParseMode.MarkdownV2,
                    //             //     cancellationToken: token);
                    //             //
                    //             // messagesSequence.States.Add(new MessageSequenceState
                    //             // {
                    //             //     Request = request,
                    //             //     InnerMessageId = message.MessageId,
                    //             //     IsSent = true
                    //             // });
                    //             
                    //             Logger.LogInformation($"Trying to replace a message '{request.Uid}': {innerMessageId}");
                    //         
                    //             message = await SendMessage<TSendOptions>(request, token, link, retText, replyMarkup, messagesSequence);
                    //             messagesSequence.States.Add(new MessageSequenceState
                    //             {
                    //                 Request = request,
                    //                 InnerMessageId = message.MessageId,
                    //                 IsSent = true
                    //             });
                    //         }
                    //     }
                    // }
                }


                if (request.Message?.Poll != default)
                {
                    var type = request.Message.Poll?.Type switch
                    {
                        Poll.PollType.Quiz => PollType.Quiz,
                        Poll.PollType.Regular => PollType.Regular,
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    message = await _client.SendPollAsync(link.chatId,
                        request.Message.Poll?.Question,
                        request.Message.Poll?.Variants,
                        isAnonymous: request.Message.Poll?.IsAnonymous,
                        type: type,
                        replyToMessageId: request.Message.ReplyToMessageUid != default
                            ? int.Parse(request.Message.ReplyToMessageUid)
                            : default,
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

                foreach (var genAttach in request.Message
                             .Attachments
                             .Where(a => a is BinaryAttachment))
                {
                    var attachment = (BinaryAttachment)genAttach;

                    switch (attachment.MediaType)
                    {
                        case MediaType.Audio:
                            var audio = new InputFileStream(attachment.Data.ToStream(), attachment.Name);
                            message = await _client.SendAudioAsync(link.chatId,
                                audio,
                                caption: request.Message.Subject,
                                parseMode: ParseMode.MarkdownV2,
                                replyToMessageId: request.Message.ReplyToMessageUid != default
                                    ? int.Parse(request.Message.ReplyToMessageUid)
                                    : default,
                                replyMarkup: replyMarkup,
                                cancellationToken: token);
                            AddChatIdInnerIdLink(response, link.chatId, message);

                            break;
                        case MediaType.Video:
                            var video = new InputFileStream(attachment.Data.ToStream(), attachment.Name);
                            message = await _client.SendVideoAsync(link.chatId,
                                video,
                                replyToMessageId: request.Message.ReplyToMessageUid != default
                                    ? int.Parse(request.Message.ReplyToMessageUid)
                                    : default,
                                replyMarkup: replyMarkup,
                                cancellationToken: token);
                            AddChatIdInnerIdLink(response, link.chatId, message);

                            break;
                        case MediaType.Image:
                            var image = new InputFileStream(attachment.Data.ToStream(), attachment.Name);
                            message = await _client.SendPhotoAsync(link.chatId,
                                image,
                                replyToMessageId: request.Message.ReplyToMessageUid != default
                                    ? int.Parse(request.Message.ReplyToMessageUid)
                                    : default,
                                replyMarkup: replyMarkup,
                                cancellationToken: token);
                            AddChatIdInnerIdLink(response, link.chatId, message);

                            break;
                        case MediaType.Voice:
                            var voice = new InputFileStream(attachment.Data.ToStream(), attachment.Name);
                            message = await _client.SendVoiceAsync(link.chatId,
                                voice,
                                caption: request.Message.Subject,
                                parseMode: ParseMode.MarkdownV2,
                                replyToMessageId: request.Message.ReplyToMessageUid != default
                                    ? int.Parse(request.Message.ReplyToMessageUid)
                                    : default,
                                replyMarkup: replyMarkup,
                                cancellationToken: token);
                            AddChatIdInnerIdLink(response, link.chatId, message);

                            break;
                        case MediaType.Sticker:
                            InputFile sticker = string.IsNullOrWhiteSpace(attachment.Url)
                                ? new InputFileStream(attachment.Data.ToStream(), attachment.Name)
                                : new InputFileUrl(attachment.Url);

                            message = await _client.SendStickerAsync(link.chatId,
                                sticker,
                                replyToMessageId: request.Message.ReplyToMessageUid != default
                                    ? int.Parse(request.Message.ReplyToMessageUid)
                                    : default,
                                replyMarkup: replyMarkup,
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
                            message = await _client.SendDocumentAsync(link.chatId,
                                doc,
                                replyToMessageId: request.Message.ReplyToMessageUid != default
                                    ? int.Parse(request.Message.ReplyToMessageUid)
                                    : default,
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

    // private async Task<Message> SendMessage<TSendOptions>(SendMessageRequest request, CancellationToken token,
    //     (string chatId, string innerId) link, string retText, IReplyMarkup replyMarkup,
    //     MessageSequenceStates messagesSequence)
    // {
    //     Message message;
    //     Logger.LogInformation(
    //         $"Message sequence number: {request.SequenceNumber}: sending a first message");
    //     message = await _client.SendTextMessageAsync(link.chatId,
    //         retText,
    //         parseMode: ParseMode.MarkdownV2,
    //         replyToMessageId: request.Message.ReplyToMessageUid != default
    //             ? int.Parse(request.Message.ReplyToMessageUid)
    //             : default,
    //         replyMarkup: replyMarkup,
    //         cancellationToken: token);
    //
    //     Logger.LogInformation(
    //         $"Message '{request.Uid}' sequence number: {request.SequenceNumber}: saving state in a cache. Inner message id {message.MessageId}");
    //
    //     messagesSequence.States.Add(new MessageSequenceState
    //     {
    //         Request = request,
    //         InnerMessageId = message.MessageId,
    //         IsSent = true
    //     });
    //     return message;
    // }

    private static void AddChatIdInnerIdLink(SendMessageResponse response, string chatId, Message message)
    {
        if (!response.Message.ChatIdInnerIdLinks.ContainsKey(chatId))
            response.Message.ChatIdInnerIdLinks[chatId] = new List<string>();

        response.Message.ChatIdInnerIdLinks[chatId].Add(message.MessageId.ToString());
    }

    private async Task SendContact(SendMessageRequest request,
        SendMessageResponse response,
        CancellationToken token,
        IReplyMarkup replyMarkup)
    {
        foreach (var chatId in request.Message?.ChatIds)
        {
            var message = await _client.SendContactAsync(chatId,
                request.Message.Contact?.Phone,
                request.Message?.Contact?.Name,
                lastName: request.Message?.Contact?.Surname,
                replyToMessageId: request.Message.ReplyToMessageUid != default
                    ? int.Parse(request.Message.ReplyToMessageUid)
                    : default,
                replyMarkup: replyMarkup,
                cancellationToken: token);

            AddChatIdInnerIdLink(response, chatId, message);
        }
    }

    /// <summary>
    ///     Autoescape for special symbols
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private static StringBuilder Escape(StringBuilder text) =>
        text.Replace("!", @"\!")
            .Replace("*", @"\*")
            .Replace("'", @"\'")
            .Replace(".", @"\.")
            .Replace("+", @"\+")
            .Replace("~", @"\~")
            .Replace("@", @"\@")
            .Replace("_", @"\_")
            .Replace("(", @"\(")
            .Replace(")", @"\)")
            .Replace("-", @"\-")
            .Replace("`", @"\`")
            .Replace("=", @"\=")
            .Replace(">", @"\>")
            .Replace("<", @"\<")
            .Replace("{", @"\{")
            .Replace("}", @"\}")
            .Replace("[", @"\[")
            .Replace("]", @"\]")
            .Replace("#", @"\#");

    /// <summary>
    ///     Starts a bot
    /// </summary>
    /// <param name="request"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    protected override async Task<StartBotResponse> InnerStartBotAsync(StartBotRequest request, CancellationToken token)
    {
        try
        {
            Logger.LogInformation($"{nameof(StartBotAsync)}...");
            var response = StartBotResponse.GetInstance(AdminCommandStatus.Ok, "");

            if (IsStarted)
            {
                Logger.LogInformation($"{nameof(StartBotAsync)}: already started");

                return response;
            }

            if (response.Status != AdminCommandStatus.Ok || IsStarted) return response;

            // Rethrowing an event from BotUpdateHandler
            _handler.MessageReceived += (sender, e)
                =>
            {
                MessageReceived?.Invoke(sender, e);
            };

            _client.StartReceiving(_handler, cancellationToken: token);

            IsStarted = true;
            Logger.LogInformation($"{nameof(StartBotAsync)}: started");

            return response;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
        }

        return StartBotResponse.GetInstance(AdminCommandStatus.Fail, "error");
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

            if (response.Status != AdminCommandStatus.Ok || !IsStarted) return response;

            await _client.CloseAsync(token);

            IsStarted = false;
            Logger.LogInformation($"{nameof(StopBotAsync)}: stopped");

            return response;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
        }

        return StopBotResponse.GetInstance(AdminCommandStatus.Fail, "error");
    }

    [Obsolete($"Use {nameof(SetBotContext)}")]
    public override async Task SetBotKey(string key, CancellationToken token)
    {
        var currentKey = _secureStorage.GetBotKey(BotDataUtils.GetBotId());

        if (currentKey?.Key != key)
        {
            var stopRequest = StopBotRequest.GetInstance();
            var startRequest = StartBotRequest.GetInstance();

            await StopBotAsync(stopRequest, token);

            _secureStorage.SetBotKey(currentKey?.Id, key);
            _client = new TelegramBotClient(key);

            await StartBotAsync(startRequest, token);
        }
    }

    public override async Task SetBotContext(BotContext context, CancellationToken token)
    {
        if (context == default) return;

        var currentContext = _secureStorage.GetBotContext(BotDataUtils.GetBotId());

        if (currentContext?.BotKey != context.BotKey)
        {
            var stopRequest = StopBotRequest.GetInstance();
            var startRequest = StartBotRequest.GetInstance();
            await StopBotAsync(stopRequest, token);

            _secureStorage.SetBotContext(context);
            _client = new TelegramBotClient(context.BotKey);

            await StartBotAsync(startRequest, token);
        }
    }
}