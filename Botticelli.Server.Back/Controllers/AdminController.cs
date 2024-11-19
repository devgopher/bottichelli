using Botticelli.Server.Back.Services;
using Botticelli.Server.Data.Entities.Bot;
using Botticelli.Shared.API.Admin.Responses;
using Botticelli.Shared.API.Client.Requests;
using Botticelli.Shared.API.Client.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Botticelli.Server.Back.Controllers;

/// <summary>
///     Admin controller getting/adding/removing bots
/// </summary>
[ApiController]
[Authorize(AuthenticationSchemes = "Bearer")]
[Route("/v1/admin")]
public class AdminController
{
    private readonly IBotManagementService _botManagementService;
    private readonly IBotStatusDataService _botStatusDataService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(IBotManagementService botManagementService,
        IBotStatusDataService botStatusDataService,
        ILogger<AdminController> logger)
    {
        _botManagementService = botManagementService;
        _botStatusDataService = botStatusDataService;
        _logger = logger;
    }

    [HttpPost("[action]")]
    public async Task<RegisterBotResponse> AddNewBot([FromBody] RegisterBotRequest request)
    {
        _logger.LogInformation($"{nameof(AddNewBot)}({request.BotId}) started...");
        var success = await _botManagementService.RegisterBot(request.BotId,
            request.BotKey,
            request.BotName,
            request.Type);

        _logger.LogInformation($"{nameof(AddNewBot)}({request.BotId}) success: {success}...");

        return new RegisterBotResponse
        {
            BotId = request.BotId,
            IsSuccess = success
        };
    }

    [HttpPut("[action]")]
    public async Task<UpdateBotResponse> UpdateBot([FromBody] UpdateBotRequest request)
    {
        _logger.LogInformation($"{nameof(UpdateBot)}({request.BotId}) started...");
        var success = await _botManagementService.UpdateBot(request.BotId,
            request.BotKey,
            request.BotName);

        _logger.LogInformation($"{nameof(UpdateBot)}({request.BotId}) success: {success}...");

        return new UpdateBotResponse
        {
            BotId = request.BotId,
            IsSuccess = success
        };
    }

    [HttpGet("[action]")]
    public async Task<ICollection<BotInfo>> GetBots()
        => _botStatusDataService.GetBots();

    [HttpGet("[action]")]
    public async Task ActivateBot([FromQuery] string botId)
        => await _botManagementService.SetRequiredBotStatus(botId, BotStatus.Unlocked);

    [HttpGet("[action]")]
    public async Task DeactivateBot([FromQuery] string botId)
        => await _botManagementService.SetRequiredBotStatus(botId, BotStatus.Locked);


    [HttpGet("[action]")]
    public async Task RemoveBot([FromQuery] string botId)
        => await _botManagementService.RemoveBot(botId);
}