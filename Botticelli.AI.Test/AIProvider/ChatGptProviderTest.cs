using System;
using System.Text.Json;
using System.Threading.Tasks;
using Botticelli.AI.ChatGpt.Message.ChatGpt;
using Botticelli.AI.ChatGpt.Provider;
using Botticelli.AI.ChatGpt.Settings;
using NUnit.Framework;
using Shared;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using Usage = Botticelli.AI.ChatGpt.Message.ChatGpt.Usage;

namespace Botticelli.AI.Test.AIProvider;

[TestFixture]
[TestOf(typeof(ChatGptProvider))]
public class ChatGptProviderTest : BaseAiProviderTest
{
    private GptSettings ChatGptSettings => new()
    {
        Url = AiSettings.Url,
        AiName = AiSettings.AiName,
        StreamGeneration = AiSettings.StreamGeneration,
        ApiKey = AiSettings.ApiKey,
        Model = "none"
    };
    
    [SetUp]
    public void StartMockServer()
    {
        Setup();
        var responseMessage = new ChatGptOutputMessage
        {
            Id = Guid.NewGuid().ToString(),
            Object = "test",
            Created = 111111,
            Model = "testapi",
            Usage = new Usage
            {
                PromptTokens = 1000,
                CompletionTokens = 1000,
                TotalTokens = 2000
            },
            Choices =
            [
                new()
                {
                    Message = new ChatGptMessage
                    {
                        Role = "test",
                        Content = ResponseString
                    },
                    Delta = null,
                    FinishReason = null,
                    Index = 0
                }
            ]
        };

        Server?.Given(Request.Create().WithPath("/completions").UsingPost())
               .RespondWith(
                            Response.Create()
                                    .WithStatusCode(200)
                                    .WithBody(JsonSerializer.Serialize(responseMessage))
                           );

        AiProvider = new ChatGptProvider(new OptionsMock<GptSettings>(ChatGptSettings),
                                               ClientFactory,
                                               LoggerMocks.CreateConsoleLogger<ChatGptProvider>(),
                                               BusClient,
                                               Validator);
    }

    [Test]
    [TestCase("test query")]
    public async Task SendAsyncTest(string query) => await InnerSendAsyncTest(query);
}