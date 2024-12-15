using Botticelli.Shared.API.Admin.Requests;
using Botticelli.Shared.API.Admin.Responses;

namespace Botticelli.Framework;

/// <summary>
///     This service interface is intended for starting/stopping your bot from
///     Api Controller
/// </summary>
public interface IBotControllerService
{
    public Task<StartBotResponse> StartAsync(StartBotRequest request);
    public Task<StopBotResponse> StopAsync(StopBotRequest request);
}