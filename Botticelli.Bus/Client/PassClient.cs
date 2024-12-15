using System.Runtime.CompilerServices;
using Botticelli.Bot.Interfaces.Client;
using Botticelli.Bus.None.Bus;
using Botticelli.Shared.API;
using Botticelli.Shared.API.Client.Requests;
using Botticelli.Shared.API.Client.Responses;

namespace Botticelli.Bus.None.Client;

public class PassClient : IBusClient
{
    private static TimeSpan Timeout => TimeSpan.FromMinutes(5);

    public async Task<SendMessageResponse> SendAndGetResponse(SendMessageRequest request,
        CancellationToken token)
    {
        NoneBus.SendMessageRequests.Enqueue(request);

        const int pause = 50;
        var waitTask = Task.Run(() =>
            {
                var period = 0;

                while (period < Timeout.TotalMilliseconds)
                {
                    if (NoneBus.SendMessageResponses.TryDequeue(out var response))
                    {
                        if (response == default) continue;

                        if (response.Uid == request.Uid) return response;
                    }

                    Task.Delay(pause, token).Wait(token);
                    period += pause;
                }

                return new SendMessageResponse(request.Uid, "Timeout")
                {
                    MessageSentStatus = MessageSentStatus.Fail
                };
            },
            token);

        return waitTask.Result;
    }

    public async IAsyncEnumerable<SendMessageResponse> SendAndGetResponseSeries(SendMessageRequest request,
        [EnumeratorCancellation] CancellationToken token)
    {
        NoneBus.SendMessageRequests.Enqueue(request);

        var period = 0;
        const int delta = 10;

        while (period < Timeout.TotalMilliseconds)
        {
            if (token.CanBeCanceled || token.IsCancellationRequested)
                yield break;

            var element = NoneBus.SendMessageResponses.Dequeue();
            if (element.MessageUid != request.Uid)
                continue;

            if (element.MessageUid == request.Uid)
            {
                yield return element;
                if (element.IsPartial == true && element.IsFinal)
                    break;
            }

            await Task.Delay(delta, token).WaitAsync(token);
            period += delta;
        }
    }

    public async Task SendResponse(SendMessageResponse response, CancellationToken tokens)
        => NoneBus.SendMessageResponses.Enqueue(response);
}