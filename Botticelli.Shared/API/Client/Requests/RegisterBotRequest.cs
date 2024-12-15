using Botticelli.Shared.Constants;

namespace Botticelli.Shared.API.Client.Requests;

public class RegisterBotRequest : IBotRequest
{
    public required string BotKey { get; set; }
    public required string BotName { get; set; }
    public BotType Type { get; set; }
    public required string BotId { get; set; }
}