using System.Text.Json;
using Botticelli.Framework.Exceptions;
using Botticelli.Framework.Vk.Messages.API.Responses;
using Botticelli.Framework.Vk.Messages.API.Utils;
using Botticelli.Framework.Vk.Messages.Options;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;

namespace Botticelli.Framework.Vk.Messages;

/// <summary>
///     Long poll method provider
///     https://dev.vk.com/ru/api/bots/getting-started
/// </summary>
public class LongPollMessagesProvider : IDisposable
{
    public delegate void GotError(VkErrorEventArgs args, CancellationToken token);

    public delegate void GotUpdates(VkUpdatesEventArgs args, CancellationToken token);

    private readonly int? _groupId = 0;

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<LongPollMessagesProvider> _logger;
    private readonly VkBotSettings _settings;
    private readonly CancellationTokenSource _tokenSource;
    private readonly object _syncObj = new();
    private string _apiKey;
    private HttpClient _client;
    private string _key;
    private int? _lastTs = 0;
    private string _server;
    private bool _isStarted;

    public LongPollMessagesProvider(VkBotSettings settings,
        IHttpClientFactory httpClientFactory,
        ILogger<LongPollMessagesProvider> logger)
    {
        _settings = settings;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _tokenSource = new CancellationTokenSource();
        _groupId = settings.GroupId;
    }

    private string ApiVersion => "5.199";

    public void Dispose()
    {
        var stopTask = Stop();
        stopTask.Wait(5000);
        _client?.Dispose();
    }

    public event GotUpdates OnUpdates;
    public event GotError OnError;


    public void SetApiKey(string key)
        => _apiKey = key;

    public async Task Start(CancellationToken token)
    {
        try
        {
            lock (_syncObj)
            {
                if (_isStarted &&
                    !string.IsNullOrWhiteSpace(_key) &&
                    !string.IsNullOrWhiteSpace(_server) &&
                    !string.IsNullOrWhiteSpace(_apiKey))
                    return;
            }

            _client = _httpClientFactory.CreateClient();

            // 1. Get Session
            await GetSessionData();

            // 2. Start polling
            if (string.IsNullOrWhiteSpace(_key) || string.IsNullOrWhiteSpace(_server))
            {
                _logger.LogError($"{nameof(_key)} or {nameof(_server)} are null or empty!");

                return;
            }


            int[] codesForRetry = [408, 500, 502, 503, 504];

            var updatePolicy = Policy
                .Handle<FlurlHttpException>(ex =>
                {
                    _logger.LogError(ex, $"Long polling error! session: {_key}, server: {_server}");

                    return codesForRetry.Contains(ex.Call.Response.StatusCode);
                })
                .WaitAndRetryAsync(3, n => n * TimeSpan.FromMilliseconds(_settings.PollIntervalMs));


            var repeatPolicy = Policy.HandleResult<UpdatesResponse>(r => true)
                .WaitAndRetryForeverAsync(_ => TimeSpan.FromMilliseconds(_settings.PollIntervalMs));
            var pollingTask = repeatPolicy.WrapAsync(updatePolicy)
                .ExecuteAsync(async () =>
                {
                    try
                    {
                        var updatesResponse = await $"{_server}".SetQueryParams(new
                            {
                                act = "a_check",
                                key = _key,
                                wait = 90,
                                ts = _lastTs,
                                mode = 2,
                                v = ApiVersion
                            })
                            .GetStringAsync(cancellationToken: _tokenSource.Token);

                        var updates = JsonSerializer.Deserialize<UpdatesResponse>(updatesResponse);


                        if (updates?.Updates == default)
                        {
                            var error = JsonSerializer.Deserialize<ErrorResponse>(updatesResponse);

                            if (error == null) return default;

                            _lastTs = error.Ts ?? _lastTs;
                            OnError?.Invoke(new VkErrorEventArgs(error), token);

                            return default;
                        }

                        _lastTs = int.Parse(updates?.Ts ?? "0");

                        if (updates?.Updates != default) OnUpdates?.Invoke(new VkUpdatesEventArgs(updates), token);

                        return updates;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Long polling error: {ex.Message}");
                    }

                    return default;
                });

            _isStarted = true;
            await pollingTask;
        }
        catch (Exception ex) when (ex is not BotException)
        {
            _logger.LogError(ex, $"Can't start a {nameof(LongPollMessagesProvider)}!");
        }
    }

    private async Task GetSessionData()
    {
        var request = new HttpRequestMessage(HttpMethod.Get,
            ApiUtils.GetMethodUri("https://api.vk.com",
                "groups.getLongPollServer",
                new
                {
                    access_token = _apiKey,
                    group_id = _groupId,
                    v = ApiVersion
                }));
        var response = await _client.SendAsync(request, _tokenSource.Token);
        var resultString = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<GetMessageSessionDataResponse>(resultString);

        _server = result?.Response?.Server;
        _key = result?.Response?.Key;
        _lastTs = int.Parse(result?.Response?.Ts ?? "0");
    }

    public async Task Stop()
    {
        _client?.CancelPendingRequests();
        _tokenSource.Cancel(false);
        _key = string.Empty;
        _server = string.Empty;
        _isStarted = false;
    }
}