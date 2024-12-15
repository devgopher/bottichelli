using Botticelli.Framework.Vk.Messages;
using Botticelli.Framework.Vk.Messages.Options;
using Botticelli.Shared.Utils;
using NUnit.Framework;
using Shared;

namespace Botticelli.Framework.Vk.Tests;

[TestFixture]
public class LongPollMessagesProviderTests
{
    [SetUp]
    public void Setup()
    {
        _provider = new LongPollMessagesProvider(new OptionsMonitorMock<VkBotSettings>(new VkBotSettings
            {
                Name = "test",
                PollIntervalMs = 500,
                GroupId = 221973506
            }).CurrentValue,
            new TestHttpClientFactory(),
            LoggerMocks.CreateConsoleLogger<LongPollMessagesProvider>());
    }

    private LongPollMessagesProvider _provider;

    [Test]
    public async Task StartTest()
    {
        _provider.NotNull();
        await _provider.Stop();
        _provider.SetApiKey(EnvironmentDataProvider.GetApiKey());

        var task = Task.Run(() => _provider.Start(CancellationToken.None));

        Thread.Sleep(5000);

        Assert.That(task.Exception == null);
    }

    [Test]
    public void StopTest()
    {
        _ = _provider.Start(CancellationToken.None);

        Thread.Sleep(2000);

        Assert.DoesNotThrowAsync(_provider.Stop);
    }
}