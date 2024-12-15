using System.Text.Json;
using Botticelli.Framework.Vk.Messages.API.Responses;
using Botticelli.Framework.Vk.Messages.API.Utils;
using RichardSzalay.MockHttp;

namespace Botticelli.Framework.Vk.Tests;

internal class TestHttpClientFactory : IHttpClientFactory
{
    public HttpClient CreateClient(string name)
    {
        var mockHttp = new MockHttpMessageHandler();

        mockHttp.When(ApiUtils.GetMethodUri("https://api.vk.com",
                "messages.send").ToString())
            .Respond("application/json", "{'Result' : 'OK'}");

        var mockResponse = new GetMessageSessionDataResponse
        {
            Response = new SessionDataResponse
            {
                Server = "https://test.mock",
                Key = "test_key",
                Ts = "12323213123"
            }
        };

        mockHttp.When(ApiUtils.GetMethodUri("https://api.vk.com", "groups.getLongPollServer").ToString())
            .Respond("application/json", JsonSerializer.Serialize(mockResponse));

        return mockHttp.ToHttpClient();
    }
}