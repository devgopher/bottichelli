namespace Botticelli.Analytics.Shared.Metrics;

/// <summary>
///     Names for typical metrics
/// </summary>
public static class MetricNames
{
    public const string MessageReceived = "MessageReceived";
    public const string MessageSent = "MessageSent";
    public const string MessageRemoved = "MessageRemoved";
    public const string NewSubscription = "NewSubscription";
    public const string UnSubscription = "UnSubscription";
    public const string BotStarted = "BotStarted";
    public const string BotStopped = "BotStopped";
    public const string CommandReceived = "CommandReceived";
    public const string BotError = "BotError";
    public const string UserDefined = "UserDefined";


    public static readonly string[] Names =
    [
        MessageReceived,
        MessageSent,
        MessageRemoved,
        BotStarted,
        BotStopped,
        NewSubscription,
        UnSubscription,
        CommandReceived,
        BotError,
        UserDefined
    ];
}