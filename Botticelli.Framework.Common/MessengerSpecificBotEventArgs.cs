using System.Collections.Generic;
using Botticelli.Interfaces;

namespace Botticelli.Framework.Events;

public class MessengerSpecificBotEventArgs<TBot> : BotEventArgs
    where TBot : IBot
{
    public required string EventName { get; set; }
    public IEnumerable<string>? Arguments { get; set; }
}