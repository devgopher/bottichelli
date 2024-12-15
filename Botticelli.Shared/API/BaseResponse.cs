namespace Botticelli.Shared.API;

public abstract class BaseResponse
{
    protected BaseResponse(string? uid, string? techMessage)
    {
        Uid = uid;
        TechMessage = techMessage;
    }

    public string? Uid { get; }

    public string? TechMessage { get; }
}

public abstract class BaseResponse<T> : BaseResponse
    where T : BaseResponse
{
    protected BaseResponse(string? uid, string? techMessage)
        : base(uid, techMessage)
    {
    }
}