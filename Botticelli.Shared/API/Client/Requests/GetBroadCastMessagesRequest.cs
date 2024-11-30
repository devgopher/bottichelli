namespace Botticelli.Shared.API.Client.Requests;

public class GetBroadCastMessagesRequest : IBotRequest
{
    public string? BotId { get; set; }
}