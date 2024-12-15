using Botticelli.Framework.Commands.Processors;
using Botticelli.Framework.Vk.Messages.API.Responses;
using Botticelli.Shared.Utils;
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

        var messagesText = botMessages?.Select(bm =>
            new
            {
                eventId = bm.EventId,
                message = bm.Object["message"]
            });

        foreach (var botMessage in messagesText.EmptyIfNull())
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
                        peerId.EmptyIfNull(),
                        fromId.EmptyIfNull()
                    ],
                    Subject = string.Empty,
                    Body = text,
                    Attachments = new List<BaseAttachment>(5),
                    From = new User
                    {
                        Id = fromId
                    },
                    ForwardedFrom = null,
                    Location = null!
                    // LastModifiedAt = botMessage.
                };

                await Process(botticelliMessage, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

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
            .GetProcessors(true)
            .Select(p => p.ProcessAsync(message, token));

        var clientChainedTasks = _processorFactory
            .GetCommandChainProcessors()
            .Select(p => p.ProcessAsync(message, token));

        Task.WaitAll(clientNonChainedTasks.Concat(clientChainedTasks).ToArray(), token);

        _logger.LogDebug($"{nameof(Process)}({message.Uid}) finished...");
        return Task.CompletedTask;
    }
}