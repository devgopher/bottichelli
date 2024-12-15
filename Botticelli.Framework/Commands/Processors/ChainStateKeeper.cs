using System.Collections.Concurrent;

namespace Botticelli.Framework.Commands.Processors;

public static class ChainStateKeeper
{
    private static readonly ConcurrentDictionary<string, bool> IsChainOpened = new();

    public static bool GetState(string chatId) =>
        IsChainOpened.TryGetValue(chatId, out var isChainOpened) && isChainOpened;

    public static void SetState(string chatId, bool isChainOpened) => IsChainOpened[chatId] = isChainOpened;

    public static void SetState(IEnumerable<string> chatIds, bool isChainOpened)
    {
        foreach (var chatId in chatIds) SetState(chatId, isChainOpened);
    }
}