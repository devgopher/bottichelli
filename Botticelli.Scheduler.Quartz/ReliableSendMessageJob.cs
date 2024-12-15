using System;
using System.Threading.Tasks;
using Botticelli.Interfaces;
using Quartz;

namespace Botticelli.Schedule.Quartz;

[DisallowConcurrentExecution]
public class ReliableSendMessageJob(IBot bot) : IJob
{
    public Task Execute(IJobExecutionContext context) => throw new NotImplementedException();
}