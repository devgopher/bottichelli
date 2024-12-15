using System;
using System.Text.Json;
using System.Threading.Tasks;
using Botticelli.AI.DeepSeekGpt.Message.DeepSeek;
using Botticelli.AI.DeepSeekGpt.Provider;
using Botticelli.AI.DeepSeekGpt.Settings;
using NUnit.Framework;
using Shared;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using Usage = Botticelli.AI.DeepSeekGpt.Message.DeepSeek.Usage;

namespace Botticelli.AI.Test.AIProvider;

[TestFixture]
[TestOf(typeof(DeepSeekGptProvider))]
public class DeepSeekGptProviderTest : BaseAiProviderTest
{
    [SetUp]
    public void StartMockServer()
    {
        Setup();
        var responseMessage = new DeepSeekOutputMessage
        {
            Id = Guid.NewGuid().ToString(),
            Object = "test",
            Created = 111111,
            Model = "testapi",
            Usage = new Usage
            {
                CompletionTokens = 1000,
                PromptTokens = 1000,
                TotalTokens = 2000
            },
            Choices =
            [
                new Choice
                {
                    DeepSeekMessage = new DeepSeekInnerOutputMessage
                    {
                        Content = ResponseString,
                        Role = "none",
                        FunctionCall = null,
                        ToolCalls = null
                    },
                    FinishReason = null,
                    Index = 0,
                    Logprobs = null
                }
            ]
        };

        Server?.Given(Request.Create().WithPath("/completions").UsingPost())
            .RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithBody(JsonSerializer.Serialize(responseMessage))
            );

        AiProvider = new DeepSeekGptProvider(new OptionsMock<DeepSeekGptSettings>(DeepSeekGptSettings),
            ClientFactory,
            LoggerMocks.CreateConsoleLogger<DeepSeekGptProvider>(),
            BusClient,
            Validator);
    }

    private DeepSeekGptSettings DeepSeekGptSettings => new()
    {
        Url = AiSettings.Url,
        AiName = AiSettings.AiName,
        StreamGeneration = AiSettings.StreamGeneration,
        ApiKey = AiSettings.ApiKey,
        Model = "none"
    };

    [Test]
    [TestCase("test query")]
    public async Task SendAsyncTest(string query) => await InnerSendAsyncTest(query);
}