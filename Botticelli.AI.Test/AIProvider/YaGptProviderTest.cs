using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Botticelli.AI.ChatGpt.Settings;
using Botticelli.AI.DeepSeekGpt.Message.DeepSeek;
using Botticelli.AI.YaGpt.Message.YaGpt;
using Botticelli.AI.YaGpt.Provider;
using Botticelli.AI.YaGpt.Settings;
using NUnit.Framework;
using Shared;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using Usage = Botticelli.AI.DeepSeekGpt.Message.DeepSeek.Usage;

namespace Botticelli.AI.Test.AIProvider;

[TestFixture]
[TestOf(typeof(YaGptProvider))]
public class YaGptProviderTest : BaseAiProviderTest
{
    private YaGptSettings YaGptSettings => new()
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
        var responseMessage = new YaGptOutputMessage
        {
            Result = new Result
            {
                Alternatives =
                [
                    new()
                    {
                        Message = new YaGpt.Message.YaGpt.Message
                        {
                            Role = "test",
                            Text = ResponseString
                        },
                        Status = "ALTERNATIVE_STATUS_TRUNCATED_FINAL"
                    }
                ]
            }
        };

        Server?.Given(Request.Create().WithPath("/completion").UsingPost())
              .RespondWith(
                           Response.Create()
                                   .WithStatusCode(200)
                                   .WithBody(JsonSerializer.Serialize(responseMessage))
                          );

        AiProvider = new YaGptProvider(new OptionsMock<YaGptSettings>(YaGptSettings),
                                       ClientFactory,
                                       LoggerMocks.CreateConsoleLogger<YaGptProvider>(),
                                       BusClient,
                                       Validator);
    }

    [Test]
    [TestCase("test query")]
    public async Task SendAsyncTest(string query) => await InnerSendAsyncTest(query);
}