using Botticelli.Shared.ValueObjects;

namespace Botticelli.Framework.Events;

public class MessageReceivedBotEventArgs : BotEventArgs
{
    public required Message Message { get; set; }
}