namespace Botticelli.Shared.API;

public abstract class BaseRequest<T> : BaseRequest
        where T : BaseRequest
{
    protected BaseRequest(string? uid)
            : base(uid)
    {
    }
}

public abstract class BaseRequest
{
    protected BaseRequest(string? uid) => Uid = uid;

    public string? Uid { get; }
}