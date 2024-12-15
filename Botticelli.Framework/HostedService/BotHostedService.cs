using Microsoft.Extensions.Hosting;

namespace Botticelli.Framework.HostedService;

/// <summary>
///     This hosted service intended for sending messages according to a schedule
/// </summary>
public class BotHostedService : IHostedService
{
    public Task StartAsync(CancellationToken token)
    {
        Console.WriteLine("Start sending messages...");

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Stop sending messages...");

        return Task.CompletedTask;
    }
}