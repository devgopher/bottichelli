using System.Globalization;
using Botticelli.Framework.Vk.Messages;
using Botticelli.Framework.Vk.Messages.API.Requests;
using NUnit.Framework;
using Shared;

namespace Botticelli.Framework.Vk.Tests;

[TestFixture]
public class MessagePublisherTests
{
    [SetUp]
    public void Setup()
    {
        _publisher = new MessagePublisher(new TestHttpClientFactory(),
            LoggerMocks.CreateConsoleLogger<MessagePublisher>());
    }

    public MessagePublisherTests()
    {
    }

    private MessagePublisher _publisher;

    public MessagePublisherTests(MessagePublisher publisher)
    {
        _publisher = publisher;
    }

    [Test]
    public async Task SendAsyncTest()
    {
        _publisher.SetApiKey(EnvironmentDataProvider.GetApiKey());
        Assert.DoesNotThrowAsync(async () => await _publisher.SendAsync(new VkSendMessageRequest
            {
                AccessToken = EnvironmentDataProvider.GetApiKey(),
                Body = $"test msg {DateTime.Now.ToString(CultureInfo.InvariantCulture)}",
                UserId = EnvironmentDataProvider.GetTargetUserId().ToString()
            },
            CancellationToken.None));
    }
}