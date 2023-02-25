﻿namespace Botticelli.Shared.API.Client.Requests;

public class PingRequest : BaseRequest<PingRequest>
{
    protected PingRequest(string uid) : base(uid)
    {
    }

    public static PingRequest GetInstance() => new(Utils.Uid.GenerateShortUid());
}