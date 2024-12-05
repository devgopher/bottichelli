namespace Botticelli.Shared.API.Client.Requests;

public class BroadCastMessagesReceivedRequest : IBotRequest
{
    public required string BotId { get; set; }
    public required string[] MessageIds { get; set; }
}