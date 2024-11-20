using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using Botticelli.Interfaces;
using Botticelli.Scheduler;
using Botticelli.Scheduler.Interfaces;
using Botticelli.Shared.API.Client.Requests;
using Botticelli.Shared.ValueObjects;
using Quartz;

namespace Botticelli.Schedule.Quartz;

public class QuartzJobManager(ISchedulerFactory schedulerFactory) : IJobManager, IDisposable
{
    private readonly CancellationTokenSource _tokenSource = new();
    private readonly List<TriggerKey> _triggerKeys = new(5);
    private IScheduler _scheduler;

    public void Dispose()
    {
        _tokenSource?.Cancel();
        _tokenSource?.Dispose();
    }

    public string AddJob(IBot bot,
        Reliability reliability,
        Message message,
        Scheduler.Schedule schedule,
        Action<Message>? preprocessFunc = default)
    {
        if (!CronExpression.IsValidExpression(schedule.Cron))
            throw new InvalidDataException($"Cron {schedule.Cron ?? "null"} is invalid!");
        
        _scheduler ??= schedulerFactory.GetScheduler().Result;

        var jobId = GetJobId();

        var request = new SendMessageRequest
        {
            Message = message
        };
        
        var serialized = JsonSerializer.Serialize(request);
        
        preprocessFunc?.Invoke(request.Message);

        var job = !reliability.IsEnabled
            ? JobBuilder.Create<SendMessageJob>()
                .WithIdentity(jobId, "sendMessageJobGroup")
                .UsingJobData("sendMessageRequest", serialized)
                .Build()
            : JobBuilder.Create<ReliableSendMessageJob>()
                .WithIdentity(jobId, "reliableSendMessageJobGroup")
                .UsingJobData("sendMessageRequest", serialized)
                .Build();
       
        var triggerId = GetTriggerIdentity();

        var trigger = TriggerBuilder.Create()
            .ForJob(job)
            .StartNow()
            .WithCronSchedule(schedule.Cron)
            .WithIdentity(triggerId)
            .StartNow()
            .Build();

        _scheduler.ScheduleJob(job, trigger, _tokenSource.Token);
        _scheduler.Start();
        _triggerKeys.Add(trigger.Key);

        return triggerId;
    }

    public void RemoveJob(string triggerId)
    {
        var existingKey = _triggerKeys.FirstOrDefault(k => k.Name == triggerId);
        if (existingKey is null)
            return;

        _scheduler.UnscheduleJob(existingKey, _tokenSource.Token);
        _triggerKeys.Remove(existingKey);
    }

    public void RemoveAllJobs()
    {
        _scheduler?.UnscheduleJobs(_triggerKeys, _tokenSource.Token);
        _triggerKeys.Clear();
    }

    private static string GetTriggerIdentity() => $"sendMessageJobGroupTrigger_{GetGuid()}";

    private static string GetGuid() => Convert.ToBase64String(Guid.NewGuid().ToByteArray());

    private static string GetJobId() => $"botticelliJob_{GetGuid()}";
}