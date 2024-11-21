using Botticelli.Analytics.Shared.Metrics;
using Botticelli.Client.Analytics;

namespace MetricsRandomGenerator;

public class MetricsSender : IHostedService
{
    private readonly MetricsPublisher _publisher;
    private readonly Random _rand = new(DateTime.Now.Microsecond);
    private readonly IServiceProvider _sp;
    private readonly int _threadsCount = 10;
    private readonly CancellationTokenSource _tokenSource = new();


    public MetricsSender(IServiceProvider sp)
    {
        _sp = sp;
        _publisher = _sp.GetRequiredService<MetricsPublisher>();
    }


    public Task StartAsync(CancellationToken cancellationToken)
    {
        for (var i = 0; i < _threadsCount; i++)
        {
            var thread = new Thread(ThreadProc);
            thread.Start();
        }

        return Task.CompletedTask;
    }


    public Task StopAsync(CancellationToken cancellationToken)
    {
        _tokenSource.Cancel();
        
        return Task.CompletedTask;
    }

    private void ThreadProc()
    {
        var token = _tokenSource.Token;
        if (!token.CanBeCanceled)
            return;

        while (!token.IsCancellationRequested)
            try
            {
                if (token.IsCancellationRequested)
                    break;

                var metric = new MetricObject
                {
                    BotId = "TestBot",
                    Name = MetricNames.Names[_rand.Next(0, MetricNames.Names.Length)],
                    Timestamp = DateTime.Now
                };

                Console.WriteLine($"Publishing: {metric.BotId}, {metric.Name}, {metric.Timestamp}");

                var task = _publisher.Publish(metric, token);
                task.Wait(token);

                Thread.Sleep(500);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
    }
}