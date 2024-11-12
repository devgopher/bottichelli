using Botticelli.Framework.Commands.Processors;
using Botticelli.Framework.Vk.Messages.API.Responses;
using Botticelli.Shared.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Botticelli.Framework.Vk.Messages.Handlers;

public class BotUpdateHandler : IBotUpdateHandler
{
    private readonly ILogger<BotUpdateHandler> _logger;
    private readonly ClientProcessorFactory _processorFactory;

    public BotUpdateHandler(ILogger<BotUpdateHandler> logger, ClientProcessorFactory processorFactory)
    {
        _logger = logger;
        _processorFactory = processorFactory;
    }

    public async Task HandleUpdateAsync(List<UpdateEvent> update, CancellationToken cancellationToken)
    {
        _logger.LogDebug($"{nameof(HandleUpdateAsync)}() started...");

        var botMessages = update?
            .Where(x => x.Type == "message_new")
            .ToList();

        var messagesText = botMessages.Select(bm =>
            new
            {
                eventId = bm.EventId,
                message = bm.Object["message"]
            }); 
        
        foreach (var botMessage in messagesText)
        {
            if (botMessage == default) continue;

            try
            {
                var eventId = botMessage.eventId;
                var peerId = botMessage.message["peer_id"]?.ToString();
                var text = botMessage.message["text"]?.ToString();
                var fromId = botMessage.message["from_id"]?.ToString();

                var botticelliMessage = new Message(eventId)
                {
                    ChatIds =
                    [
                        peerId,
                        fromId
                    ],
                    Subject = string.Empty,
                    Body = text,
                    Attachments = new List<BaseAttachment>(5),
                    From = new User
                    {
                        Id = fromId
                    },
                    ForwardedFrom = null,
                    Location = null,
                    // LastModifiedAt = botMessage.
                };

                await Process(botticelliMessage, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(HandleUpdateAsync)}() error", ex);
            }
        }


        //if (botMessage == null) return;

        //var botticelliMessage = new Message(botMessage.MessageId.ToString())
        //{
        //    ChatIds = new() {botMessage.Chat.Id.ToString()},
        //    Subject = string.Empty,
        //    Body = botMessage.Text,
        //    Attachments = new List<IAttachment>(5),
        //    From = new User
        //    {
        //        Id = botMessage.From?.Id.ToString(),
        //        Name = botMessage.From?.FirstName,
        //        Surname = botMessage.From?.LastName,
        //        Info = string.Empty,
        //        IsBot = botMessage.From?.IsBot,
        //        NickName = botMessage.From?.Username
        //    },
        //    ForwardedFrom = new User
        //    {
        //        Id = botMessage.ForwardFrom?.Id.ToString(),
        //        Name = botMessage.ForwardFrom?.FirstName,
        //        Surname = botMessage.ForwardFrom?.LastName,
        //        Info = string.Empty,
        //        IsBot = botMessage.ForwardFrom?.IsBot,
        //        NickName = botMessage.ForwardFrom?.Username
        //    },
        //    Location = botMessage.Location != null ?
        //            new GeoLocation
        //            {
        //                Latitude = (decimal) botMessage.Location?.Latitude,
        //                Longitude = (decimal) botMessage.Location?.Longitude
        //            } :
        //            null
        //};

        //await Process(botticelliMessage, cancellationToken);

        _logger.LogDebug($"{nameof(HandleUpdateAsync)}() finished...");
    }

    public event IBotUpdateHandler.MsgReceivedEventHandler? MessageReceived;

    /// <summary>
    ///     Processes requests
    /// </summary>
    /// <param name="message"></param>
    /// <param name="token"></param>
    protected Task Process(Message message, CancellationToken token)
    {
        _logger.LogDebug($"{nameof(Process)}({message.Uid}) started...");

        if (token is { CanBeCanceled: true, IsCancellationRequested: true }) return Task.CompletedTask;

        var clientNonChainedTasks = _processorFactory
            .GetProcessors(excludeChain: true)
            .Select(p => p.ProcessAsync(message, token));

        var clientChainedTasks = _processorFactory
                                    .GetCommandChainProcessors()
                                    .Select(p => p.ProcessAsync(message, token));

        Task.WaitAll(clientNonChainedTasks.Concat(clientChainedTasks).ToArray(), token);

        _logger.LogDebug($"{nameof(Process)}({message.Uid}) finished...");
        return Task.CompletedTask;
    }
}