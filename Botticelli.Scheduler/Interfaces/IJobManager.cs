using Botticelli.Interfaces;
using Botticelli.Shared.ValueObjects;

namespace Botticelli.Scheduler.Interfaces;

public interface IJobManager
{
    string AddJob(IBot bot,
        Reliability reliability,
        Message message,
        Schedule schedule,
        Action<Message>? preprocessFunc = default);

    void RemoveJob(string triggerId);
    void RemoveAllJobs();
}