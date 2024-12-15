using Botticelli.Bot.Interfaces.Client;
using Botticelli.Bus.None.Bus;
using Botticelli.Shared.API.Client.Requests;

namespace Botticelli.Bus.None.Client;

public class PassEventClient : IEventBusClient
{
    private const int Pause = 5;
    private readonly Task _workerTask;
    private bool _startedFlag = true;

    public PassEventClient()
    {
        _workerTask = Task.Run(Process);
    }

    public event IEventBusClient.BusEventHandler OnReceived;

    public Task Send(SendMessageRequest request, CancellationToken token)
    {
        NoneBus.SendMessageRequests.Enqueue(request);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _startedFlag = false;
        _workerTask.Wait(500);
    }

    private void Process()
    {
        while (_startedFlag)
        {
            if (NoneBus.SendMessageResponses.TryDequeue(out var message))
                OnReceived?.Invoke(this, message);

            Thread.Sleep(Pause);
        }
    }
}