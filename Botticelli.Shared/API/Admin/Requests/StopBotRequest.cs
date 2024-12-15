using Botticelli.Shared.Utils;

namespace Botticelli.Shared.API.Admin.Requests;

public class StopBotRequest : BaseRequest<StopBotRequest>
{
    protected StopBotRequest(string? uid) : base(uid)
    {
    }

    public static StopBotRequest GetInstance() => new(BotIdUtils.GenerateShortBotId());
}