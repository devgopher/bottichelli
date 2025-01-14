namespace Botticelli.Shared.ValueObjects;

/// <summary>
///     Received/sent message
/// </summary>
[Serializable]
public class Message
{
    /// <summary>
    ///     Type of message
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        ///     A simple texting message
        /// </summary>
        Messaging,

        /// <summary>
        ///     A command message
        /// </summary>
        Command,

        /// <summary>
        ///     A specific type with some type extensions
        /// </summary>
        Extended
    }

    public Message()
    {
        Uid = Guid.NewGuid().ToString();
        CreatedAt = DateTime.Now;
        ProcessingArgs = new List<string>(1);
    }

    public Message(string uid)
    {
        Uid = uid;
        CreatedAt = DateTime.Now;
        ProcessingArgs = new List<string>(1);
    }

    /// <summary>
    ///     Message type
    /// </summary>
    public virtual MessageType Type { get; set; }

    /// <summary>
    ///     Message uid
    /// </summary>
    public string? Uid { get; set; }

    /// <summary>
    ///     Chat Id <=> Inner message id links
    /// </summary>
    public Dictionary<string, List<string>> ChatIdInnerIdLinks { get; init; } = new();

    /// <summary>
    ///     Chat ids
    /// </summary>
    public List<string> ChatIds { get; set; }

    /// <summary>
    ///     Message subj
    /// </summary>
    public string? Subject { get; set; }

    /// <summary>
    ///     Message body
    /// </summary>
    public string? Body { get; set; }

    /// <summary>
    ///     Message arguments for processing
    /// </summary>
    public IList<string>? ProcessingArgs { get; set; }

    /// <summary>
    ///     Message attachments
    /// </summary>
    public List<BaseAttachment>? Attachments { get; set; }

    /// <summary>
    ///     From user
    /// </summary>
    public User? From { get; set; }

    /// <summary>
    ///     Forwarded from user
    /// </summary>
    public User? ForwardedFrom { get; set; }

    /// <summary>
    ///     Contacts
    /// </summary>
    public Contact? Contact { get; set; }

    /// <summary>
    ///     Poll
    /// </summary>
    public Poll? Poll { get; set; }

    /// <summary>
    ///     Reply to a message with uid
    /// </summary>
    public string? ReplyToMessageUid { get; set; }

    /// <summary>
    ///     GeoLocation
    /// </summary>
    public GeoLocation Location { get; set; }

    /// <summary>
    ///     Callback data if exists
    /// </summary>
    public string? CallbackData { get; set; }

    /// <summary>
    ///     Message creation date
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    ///     Message modification date
    /// </summary>
    public DateTime LastModifiedAt { get; set; }

    /// <summary>
    ///     Chain id for chained command processing
    /// </summary>
    public Guid? ChainId { get; set; }
}