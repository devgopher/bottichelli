using Botticelli.Shared.Utils;
using Botticelli.Shared.ValueObjects;

namespace Botticelli.Shared.API.Client.Responses;

public class SendMessageResponse : BaseResponse<SendMessageResponse>
{
    public SendMessageResponse(string? uid, string? techMessage = null) : base(uid, techMessage)
    {
        Message = new Message();
    }

    public string? MessageUid { get; set; }

    public MessageSentStatus MessageSentStatus { get; set; }

    public Message Message { get; set; }

    public static SendMessageResponse GetInstance(string? techMessage) =>
        new(BotIdUtils.GenerateShortBotId(), techMessage);

    public static SendMessageResponse GetInstance(string? uid, string? techMessage) => new(uid, techMessage);

    #region PartialResponses

    /// <summary>
    ///     Is this a partial response?
    /// </summary>
    public bool? IsPartial { get; set; }

    /// <summary>
    ///     Sub-id for a partial response.
    ///     Example: {MessageUid}:{SequenceNumber}
    /// </summary>
    public int SequenceNumber { get; set; }

    /// <summary>
    ///     Is it a last part of a partial response?
    /// </summary>
    public bool IsFinal { get; set; }

    #endregion
}