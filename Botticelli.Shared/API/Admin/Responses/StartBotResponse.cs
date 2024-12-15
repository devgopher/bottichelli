using Botticelli.Shared.Utils;

namespace Botticelli.Shared.API.Admin.Responses;

public class StartBotResponse : BaseResponse<StartBotResponse>
{
    protected StartBotResponse(string? uid, string techMessage, AdminCommandStatus status)
        : base(uid, techMessage)
    {
        Status = status;
    }

    public AdminCommandStatus Status { get; }

    public static StartBotResponse GetInstance(AdminCommandStatus status, string techMessage) =>
        new(BotIdUtils.GenerateShortBotId(), techMessage, status);

    public static StartBotResponse GetInstance(string? uid, string techMessage, AdminCommandStatus status) =>
        new(uid, techMessage, status);
}