using Botticelli.Shared.ValueObjects;

namespace Botticelli.Framework.Events;

public class MessageSentBotEventArgs : BotEventArgs
{
    public required Message Message { get; set; }
}