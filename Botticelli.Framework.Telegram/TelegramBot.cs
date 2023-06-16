﻿using System.Text;
using Botticelli.Framework.Events;
using Botticelli.Framework.Exceptions;
using Botticelli.Framework.SendOptions;
using Botticelli.Framework.Telegram.Handlers;
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
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace Botticelli.Framework.Telegram;

public class TelegramBot : BaseBot<TelegramBot>
{
    private readonly ITelegramBotClient _client;
    private readonly IBotUpdateHandler _handler;
    private readonly ILogger<TelegramBot> _logger;

    /// <summary>
    ///     ctor
    /// </summary>
    /// <param name="client"></param>
    /// <param name="handler"></param>
    /// <param name="logger"></param>
    public TelegramBot(ITelegramBotClient client, IBotUpdateHandler handler, ILogger<TelegramBot> logger) : base(logger)
    {
        IsStarted = false;
        _client = client;
        _handler = handler;
        _logger = logger;
    }

    public override BotType Type => BotType.Telegram;
    public virtual event MsgSentEventHandler MessageSent;
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
    public override async Task<RemoveMessageResponse> DeleteMessageAsync(RemoveMessageRequest request, CancellationToken token)
    {
        if (!IsStarted)
        {
            _logger.LogInformation("Bot wasn't started!");

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
        catch (Exception ex)
        {
            response.MessageRemovedStatus = MessageRemovedStatus.Fail;
        }

        response.MessageUid = request.Uid;

        MessageRemoved?.Invoke(this,
                               new MessageRemovedBotEventArgs
                               {
                                   MessageUid = request.Uid
                               });

        return response;
    }

    /// <summary>
    ///     Sends a message as a telegram bot
    /// </summary>
    /// <param name="request"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="BotException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public override async Task<SendMessageResponse> SendMessageAsync<TSendOptions>(SendMessageRequest request,
                                                                                   SendOptionsBuilder<TSendOptions> optionsBuilder,
                                                                                   CancellationToken token)
    {
        if (!IsStarted)
        {
            _logger.LogInformation("Bot wasn't started!");

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

            text = Escape(text);
            var retText = text.ToString();

            if (!string.IsNullOrWhiteSpace(retText))
                await _client.SendTextMessageAsync(request.Message.ChatId,
                                                   retText,
                                                   ParseMode.MarkdownV2,
                                                   replyToMessageId: request.Message.ReplyToMessageUid != default ? int.Parse(request.Message.ReplyToMessageUid) : default,
                                                   replyMarkup: replyMarkup,
                                                   cancellationToken: token);

            if (request.Message?.Poll != default)
            {
                var type = request.Message.Poll?.Type switch
                {
                    Poll.PollType.Quiz    => PollType.Quiz,
                    Poll.PollType.Regular => PollType.Regular,
                    _                     => throw new ArgumentOutOfRangeException()
                };

                await _client.SendPollAsync(request.Message.ChatId,
                                            request.Message.Poll?.Question,
                                            request.Message.Poll?.Variants,
                                            request.Message.Poll?.IsAnonymous,
                                            type,
                                            replyToMessageId: request.Message.ReplyToMessageUid != default ? int.Parse(request.Message.ReplyToMessageUid) : default,
                                            replyMarkup: replyMarkup,
                                            cancellationToken: token);
            }

            if (request.Message?.Contact != default)
                await _client.SendContactAsync(request.Message.ChatId,
                                               request.Message.Contact?.Phone,
                                               request.Message.Contact?.Name,
                                               request.Message.Contact?.Surname,
                                               replyToMessageId: request.Message.ReplyToMessageUid != default ? int.Parse(request.Message.ReplyToMessageUid) : default,
                                               replyMarkup: replyMarkup,
                                               cancellationToken: token);

            if (request.Message.Attachments != null)
                foreach (var genAttach in request.Message.Attachments.Where(a => a is BinaryAttachment))
                {
                    var attachment = (BinaryAttachment) genAttach;

                    switch (attachment.MediaType)
                    {
                        case MediaType.Audio:
                            var audio = new InputOnlineFile(attachment.Data.ToStream(), attachment.Name);
                            await _client.SendAudioAsync(request.Message.ChatId,
                                                         audio,
                                                         request.Message.Subject,
                                                         ParseMode.MarkdownV2,
                                                         replyToMessageId: request.Message.ReplyToMessageUid != default ? int.Parse(request.Message.ReplyToMessageUid) : default,
                                                         replyMarkup: replyMarkup,
                                                         cancellationToken: token);

                            break;
                        case MediaType.Video:
                            var video = new InputOnlineFile(attachment.Data.ToStream(), attachment.Name);
                            await _client.SendVideoAsync(request.Message.ChatId,
                                                         video,
                                                         replyToMessageId: request.Message.ReplyToMessageUid != default ? int.Parse(request.Message.ReplyToMessageUid) : default,
                                                         replyMarkup: replyMarkup,
                                                         cancellationToken: token);

                            break;
                        case MediaType.Image:
                            var image = new InputOnlineFile(attachment.Data.ToStream(), attachment.Name);
                            await _client.SendPhotoAsync(request.Message.ChatId,
                                                         image,
                                                         replyToMessageId: request.Message.ReplyToMessageUid != default ? int.Parse(request.Message.ReplyToMessageUid) : default,
                                                         replyMarkup: replyMarkup,
                                                         cancellationToken: token);

                            break;
                        case MediaType.Voice:
                            var voice = new InputOnlineFile(attachment.Data.ToStream(), attachment.Name);
                            await _client.SendVoiceAsync(request.Message.ChatId,
                                                         voice,
                                                         request.Message.Subject,
                                                         ParseMode.MarkdownV2,
                                                         replyToMessageId: request.Message.ReplyToMessageUid != default ? int.Parse(request.Message.ReplyToMessageUid) : default,
                                                         replyMarkup: replyMarkup,
                                                         cancellationToken: token);

                            break;
                        case MediaType.Sticker:
                            var sticker = string.IsNullOrWhiteSpace(attachment.Url) ?
                                    new InputOnlineFile(attachment.Data.ToStream(), attachment.Name) :
                                    new InputOnlineFile(attachment.Url);

                            await _client.SendStickerAsync(request.Message.ChatId,
                                                           sticker,
                                                           replyToMessageId: request.Message.ReplyToMessageUid != default ? int.Parse(request.Message.ReplyToMessageUid) : default,
                                                           replyMarkup: replyMarkup,
                                                           cancellationToken: token);

                            break;
                        case MediaType.Unknown:
                        case MediaType.Contact:
                        case MediaType.Poll:
                        case MediaType.Text:
                            // nothing to do
                            break;
                    }
                }

            response.MessageSentStatus = MessageSentStatus.Ok;

            MessageSent?.Invoke(this,
                                new MessageSentBotEventArgs
                                {
                                    Message = request.Message
                                });
        }
        catch (Exception ex)
        {
            response.MessageSentStatus = MessageSentStatus.Fail;
            _logger.LogError(ex, ex.Message);
        }

        return response;
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
                .Replace("]", @"\]");

    /// <summary>
    ///     Starts a bot
    /// </summary>
    /// <param name="request"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public override async Task<StartBotResponse> StartBotAsync(StartBotRequest request, CancellationToken token)
    {
        try
        {
            _logger.LogInformation($"{nameof(StartBotAsync)}...");
            var response = await base.StartBotAsync(request, token);

            if (IsStarted)
            {
                _logger.LogInformation($"{nameof(StartBotAsync)}: already started");

                return response;
            }

            if (response.Status != AdminCommandStatus.Ok || IsStarted) return response;

            // Rethrowing an event from BotUpdateHandler
            _handler.MessageReceived += (sender, e)
                    => MessageReceived?.Invoke(sender, e);

            _client.StartReceiving(_handler, cancellationToken: token);

            IsStarted = true;
            _logger.LogInformation($"{nameof(StartBotAsync)}: started");

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }

        return StartBotResponse.GetInstance(AdminCommandStatus.Fail, "error");
    }

    /// <summary>
    ///     Stops a bot
    /// </summary>
    /// <param name="request"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public override async Task<StopBotResponse> StopBotAsync(StopBotRequest request, CancellationToken token)
    {
        try
        {
            _logger.LogInformation($"{nameof(StopBotAsync)}...");
            var response = await base.StopBotAsync(request, token);

            if (response.Status != AdminCommandStatus.Ok || !IsStarted) return response;

            await _client.CloseAsync(token);

            IsStarted = false;
            _logger.LogInformation($"{nameof(StopBotAsync)}: stopped");

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }

        return StopBotResponse.GetInstance(AdminCommandStatus.Fail, "error");
    }
}