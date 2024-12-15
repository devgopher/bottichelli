namespace Botticelli.Server.Back.Services.Auth;

public interface IPasswordSender
{
    public Task SendPassword(string email, string password, CancellationToken ct);
}