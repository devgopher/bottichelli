using Botticelli.Shared.Utils;
using Botticelli.Shared.ValueObjects;

namespace Botticelli.Shared.API.Client.Requests;

public class SendMessageRequest : BaseRequest<SendMessageRequest>
{
    public SendMessageRequest() : this(Guid.NewGuid().ToString())
    {
    }

    public SendMessageRequest(string? uid) : base(uid) => Message = new Message(uid);

    public bool? ExpectPartialResponse { get; set; }
    public int? SequenceNumber { get; set; }
    public bool? IsFinal { get; set; }
    public Message Message { get; set; }
}