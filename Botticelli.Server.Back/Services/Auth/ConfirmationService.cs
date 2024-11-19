using System.Text;
using Botticelli.Server.Settings;
using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using Flurl;
using Microsoft.AspNetCore.Identity;

namespace Botticelli.Server.Back.Services.Auth;

public class ConfirmationService : IConfirmationService
{
    private readonly ISender _fluentEmail;
    private readonly ServerSettings _serverSettings;
    private readonly UserManager<IdentityUser<string>> _userManager;

    public ConfirmationService(ISender fluentEmail, UserManager<IdentityUser<string>> userManager,
        ServerSettings serverSettings)
    {
        _fluentEmail = fluentEmail;
        _userManager = userManager;
        _serverSettings = serverSettings;
    }

    public async Task SendConfirmationCode(IdentityUser<string> user, CancellationToken ct)
    {
        var srcToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var token = Convert.ToBase64String(Encoding.UTF8.GetBytes(srcToken));

        var message = Email.From(_serverSettings.ServerEmail, "BotticelliBots Admin Service")
            .To(user.Email)
            .Subject("BotticelliBots email confirmation")
            .Body(
                $"Confirmation email link: {Url.Combine(_serverSettings.ServerUrl, "/user/ConfirmEmail").SetQueryParam("Email", user.Email).SetQueryParam("Token", token)}");

        var sendResult = await _fluentEmail.SendAsync(message, ct);

        if (!sendResult.Successful)
            throw new InvalidOperationException($"Sending mail errors:  {string.Join(',', sendResult.ErrorMessages)}");
    }


    public async Task<bool> ConfirmCodeAsync(string srcToken, IdentityUser<string> user, CancellationToken ct)
    {
        var token = Encoding.UTF8.GetString(Convert.FromBase64String(srcToken));

        var result = await _userManager.ConfirmEmailAsync(user, token);

        return result.Succeeded;
    }
}