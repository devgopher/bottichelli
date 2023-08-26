﻿using BotDataSecureStorage.Settings;
using Botticelli.Framework.Vk.Options;
using Botticelli.Framework.Vk.Tests.Settings;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Botticelli.Framework.Vk.Tests;

[TestFixture]
public class LongPollMessagesProviderTests
{
    [SetUp]
    public void Setup()
    {
        var config = new ConfigurationBuilder()
                     .AddJsonFile("appsettings.json")
                     .Build();

        var settings = config.GetSection(nameof(SampleSettings))
                             .Get<SampleSettings>();

        _provider = new LongPollMessagesProvider(new OptionsMonitorMock<VkBotSettings>(new VkBotSettings
                                                 {
                                                     SecureStorageSettings = new SecureStorageSettings
                                                     {
                                                         ConnectionString = settings.SecureStorageConnectionString
                                                     },
                                                     Name = "test",
                                                     PollIntervalMs = 500,
                                                     GroupId = 221973506
                                                 }),
                                                 new TestHttpClientFactory(),
                                                 Utils.CreateConsoleLogger<LongPollMessagesProvider>());
    }

    private LongPollMessagesProvider _provider;

    [Test]
    public async Task StartTest()
    {
        _provider.SetApiKey(EnvironmentDataProvider.GetApiKey()); 
        
        var task = Task.Run(() => _provider.Start());

        Thread.Sleep(5000);

        Assert.IsNull(task.Exception);
    }

    [Test]
    public void StopTest()
    {
        Assert.DoesNotThrowAsync(_provider.Stop);
    }
}