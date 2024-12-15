using Botticelli.Shared.Constants;

namespace Botticelli.Shared.API.Client.Requests;

public class RegisterBotRequest
{
    public required string BotId { get; set; }
    public required string BotKey { get; set; }
    public required string BotName { get; set; }
    public BotType Type { get; set; }
}