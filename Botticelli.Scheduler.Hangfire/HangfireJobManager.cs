using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Botticelli.Interfaces;
using Botticelli.Scheduler.Interfaces;
using Botticelli.Shared.API.Client.Requests;
using Botticelli.Shared.ValueObjects;
using Hangfire;

namespace Botticelli.Scheduler.Hangfire;

public class HangfireJobManager : IJobManager
{
    private readonly List<string> _jobIds = new(5);

    public string AddJob(IBot bot,
        Reliability reliability,
        Message message,
        Scheduler.Schedule schedule,
        Action<Message>? preprocessFunc = default)
    {
        var jobId = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

        var request = new SendMessageRequest
        {
            Message = message
        };

        preprocessFunc?.Invoke(request.Message);

        if (!reliability.IsEnabled)
        {
            RecurringJob.AddOrUpdate(jobId,
                () => bot.SendMessageAsync(request, CancellationToken.None),
                schedule.Cron);
            _jobIds.Add(jobId);

            return jobId;
        }

        RecurringJob.AddOrUpdate(jobId,
            () => SendWithReliability(bot,
                request,
                reliability,
                CancellationToken.None),
            schedule.Cron);
        _jobIds.Add(jobId);

        return jobId;
    }

    public void RemoveJob(string triggerId)
    {
        RecurringJob.RemoveIfExists(triggerId);
        _jobIds.Remove(triggerId);
    }

    public void RemoveAllJobs()
    {
        foreach (var jobId in _jobIds) RecurringJob.RemoveIfExists(jobId);

        _jobIds.Clear();
    }

    public async Task SendWithReliability(IBot bot,
        SendMessageRequest request,
        Reliability reliability,
        CancellationToken token)
        => throw new NotImplementedException();
}