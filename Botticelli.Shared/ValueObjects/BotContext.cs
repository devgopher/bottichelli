using LiteDB;

namespace Botticelli.Shared.ValueObjects;

public class BotContext
{
    /// <summary>
    ///     Botticelli bot id
    /// </summary>
    [BsonId]
    public required string BotId { get; set; }

    /// <summary>
    ///     Bot key in a messenger
    /// </summary>
    public required string BotKey { get; set; }

    /// <summary>
    ///     Additional info
    /// </summary>
    public Dictionary<string, string>? Items { get; set; }
}