using Botticelli.Shared.ValueObjects;

namespace Botticelli.Shared.API.Client.Requests;

public class RemoveMessageRequest : BaseRequest<RemoveMessageRequest>
{
    public RemoveMessageRequest(Message message) : base(message.Uid) => Message = message;

    public Message Message { get; set; }
}