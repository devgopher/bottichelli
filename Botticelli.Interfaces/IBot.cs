using Botticelli.Shared.Constants;

namespace Botticelli.Interfaces;

/// <summary>
///     Common interface for bots
/// </summary>
public interface IBot : IEventBasedBotAdminApi, IEventBasedBotClientApi
{
    BotType Type { get; }
    string? BotUserId { get; protected set; }
}

/// <summary>
///     Common interface for bots
/// </summary>
public interface IBot<T> : IBot
    where T : IBot<T>
{
}