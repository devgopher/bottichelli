using System;
using System.Threading;
using System.Threading.Tasks;
using Botticelli.AI.AIProvider;
using Botticelli.AI.ChatGpt.Settings;
using Botticelli.AI.DeepSeekGpt.Settings;
using Botticelli.AI.Message;
using Botticelli.AI.Settings;
using Botticelli.AI.Validation;
using Botticelli.Bot.Interfaces.Client;
using Botticelli.Bus.None.Bus;
using Botticelli.Bus.None.Client;
using Botticelli.Shared.Utils;
using FluentAssertions;
using FluentValidation;
using NUnit.Framework;
using Shared;
using WireMock.Server;

namespace Botticelli.AI.Test.AIProvider;

public abstract class BaseAiProviderTest 
{
    protected WireMockServer? Server;
    protected IAiProvider? AiProvider;
    protected AbstractValidator<AiMessage>? Validator;
    protected IBusClient? BusClient;
    protected readonly AiSettings AiSettings = new()
    {
        AiName = "mock_gpt_ai",
        StreamGeneration = false,
        ApiKey = "API9767432",
    };

    protected HttpClientFactoryMock? ClientFactory;


    protected const string ResponseString = "\"Wololo\" is the battle cry of the Priest unit featured in the 1997 historical real-time strategy " +
                                            "game Age of Empires. Due to its association with the Priest's mystical ability to assume control of an opponent's unit" +
                                            " through conversion, the sound effect has gained notoriety among the fans as one of the most dreaded stock lines " +
                                            "from the game.";

    protected async Task InnerSendAsyncTest(string query)
    {
        var message = new AiMessage
        {
            Type = Shared.ValueObjects.Message.MessageType.Messaging,
            Uid = Guid.NewGuid().ToString(),
            Subject = string.Empty,
            Body = query
        };

        AiProvider.NotNull();
        
        await AiProvider.SendAsync(message, new CancellationToken());
        
        Thread.Sleep(5000);
        var result = NoneBus.SendMessageResponses.Dequeue();

        result.Should().NotBeNull();
        result.Message.Should().NotBeNull();
        result.Message.Body.Should().NotBeEmpty();
    }
    
    protected void Setup()
    {
        Server = WireMockServer.Start();
        Validator = new AiMessageValidator();
        BusClient = new PassClient();
        
        ClientFactory = new HttpClientFactoryMock();
        
        Server.Url.NotNull();
        AiSettings.Url = Server.Url;
    }
    
    [TearDown]
    public void TearDown()
    {
        Server.NotNull();
        Server.Stop();
    }
}