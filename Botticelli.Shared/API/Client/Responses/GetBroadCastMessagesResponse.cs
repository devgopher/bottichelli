using Botticelli.Shared.API.Admin.Responses;
using Botticelli.Shared.ValueObjects;

namespace Botticelli.Shared.API.Client.Responses;

public class GetBroadCastMessagesResponse : ServerBaseResponse
{
    public required string BotId { get; set; }
    public required Message[] Messages { get; set; }
}