namespace Shared;

public class HttpClientFactoryMock : IHttpClientFactory
{
    private HttpClient? _client;
    public Uri? BaseAddress { get; set; }

    public HttpClient CreateClient(string name)
    { 
        _client ??= new HttpClient();
        _client.BaseAddress = BaseAddress;

        return _client;
    }
}