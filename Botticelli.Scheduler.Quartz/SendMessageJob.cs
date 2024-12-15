using System;
using System.Text.Json;
using System.Threading.Tasks;
using Botticelli.Interfaces;
using Botticelli.Shared.API.Client.Requests;
using Quartz;

namespace Botticelli.Schedule.Quartz;

[DisallowConcurrentExecution]
public class SendMessageJob(IBot bot) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var request = context.JobDetail?.JobDataMap.Get("sendMessageRequest").ToString();
        if (request != null)
        {
            var sendMessageRequest = JsonSerializer.Deserialize<SendMessageRequest>(request);

            if (sendMessageRequest is null)
                throw new NullReferenceException($"{nameof(sendMessageRequest)} is null!");

            await bot.SendMessageAsync(sendMessageRequest, context.CancellationToken);
        }
    }
}