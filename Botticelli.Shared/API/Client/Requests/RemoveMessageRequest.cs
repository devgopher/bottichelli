namespace Botticelli.Shared.API.Client.Requests;

public class RemoveMessageRequest : BaseRequest<RemoveMessageRequest>
{
    public RemoveMessageRequest(string? uid, string chatId) : base(uid)
    {
        ChatId = chatId;
    }

    public string? ChatId { get; set; }
}