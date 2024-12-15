using Botticelli.Shared.Utils;

namespace Botticelli.Shared.API.Client.Responses;

public class RemoveMessageResponse : BaseResponse<RemoveMessageResponse>
{
    public RemoveMessageResponse(string? uid, string? techMessage) : base(uid, techMessage)
    {
    }

    public string? MessageUid { get; set; }

    public MessageRemovedStatus MessageRemovedStatus { get; set; }

    public static RemoveMessageResponse GetInstance(string? techMessage) =>
        new(BotIdUtils.GenerateShortBotId(), techMessage);

    public static RemoveMessageResponse GetInstance(string? uid, string? techMessage) => new(uid, techMessage);
}