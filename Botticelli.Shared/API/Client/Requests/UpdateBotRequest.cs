namespace Botticelli.Shared.API.Client.Requests;

public class UpdateBotRequest
{
    public required string BotId { get; init; }
    public required string BotKey { get; init; }
    public required string BotName { get; init; }
}