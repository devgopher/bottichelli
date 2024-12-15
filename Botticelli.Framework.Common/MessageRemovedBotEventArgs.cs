namespace Botticelli.Framework.Events;

public class MessageRemovedBotEventArgs : BotEventArgs
{
    public required string MessageUid { get; set; }
}