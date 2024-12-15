using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Viber.Api.Exceptions;
using Viber.Api.Requests;
using Viber.Api.Responses;
using Viber.Api.Settings;

namespace Viber.Api
{
    public class ViberService : IViberService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpListener _httpListener;
        private readonly ViberApiSettings _settings;
        private readonly ManualResetEventSlim _stopReceiving = new ManualResetEventSlim(false);

        public ViberService(IHttpClientFactory httpClientFactory,
            ViberApiSettings settings)
        {
            _httpListener = new HttpListener();
            _httpClientFactory = httpClientFactory;
            _settings = settings;
            _httpListener.Prefixes.Add("http://127.0.0.1:5000/");

            Start();
        }

        public event IViberService.GotMessageHandler? GotMessage;

        public void Start()
        {
            _httpListener.Start();
            Task.Run(() => ReceiveMessages());

            _ = SetWebHook(new SetWebHookRequest
            {
                Url = _settings.HookUrl,
                AuthToken = _settings.ViberToken,
                EventTypes = new List<string>
                {
                    "delivered",
                    "seen",
                    "failed",
                    "subscribed",
                    "unsubscribed",
                    "conversation_started"
                }
            });
        }

        public void Stop()
        {
            _stopReceiving.Set();
            _httpListener.Stop();
        }

        public async Task<SetWebHookResponse> SetWebHook(SetWebHookRequest request,
            CancellationToken cancellationToken = default)
        {
            request.AuthToken = _settings.ViberToken;

            return await InnerSend<SetWebHookRequest, SetWebHookResponse>(request,
                "set_webhook",
                cancellationToken);
        }

        public async Task<ApiSendMessageResponse> SendMessage(ApiSendMessageRequest request,
            CancellationToken cancellationToken = default)
        {
            request.AuthToken = _settings.ViberToken;

            return await InnerSend<ApiSendMessageRequest, ApiSendMessageResponse>(request,
                "send_message",
                cancellationToken);
        }

        public void Dispose()
        {
            Stop();
            _httpListener.Close();
        }

        protected async Task ReceiveMessages(CancellationToken cancellationToken = default)
        {
            while (!_stopReceiving.IsSet)
                try
                {
                    if (cancellationToken is { CanBeCanceled: true, IsCancellationRequested: true })
                    {
                        _stopReceiving.Set();

                        return;
                    }

                    if (!_httpListener.IsListening) continue;

                    var context = await _httpListener.GetContextAsync();

                    if (context.Response.StatusCode == (int)HttpStatusCode.OK)
                    {
                        using var sr = new StreamReader(context.Response.OutputStream);
                        var content = await sr.ReadToEndAsync();
                        var deserialized = JsonSerializer.Deserialize<GetWebHookEvent>(content);

                        if (deserialized == default) continue;

                        GotMessage?.Invoke(deserialized);
                    }
                    else
                    {
                        throw new ViberClientException($"Error listening: {context.Response.StatusCode}, " +
                                                       $"{context.Response.StatusDescription}");
                    }
                }
                catch (Exception ex)
                {
                    throw new ViberClientException(ex.Message, ex);
                }
        }

        private async Task<TResp> InnerSend<TReq, TResp>(TReq request,
            string funcName,
            CancellationToken cancellationToken)
        {
            using var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri( /*_settings.RemoteUrl*/ _settings.HookUrl);

            var content = JsonContent.Create(request);

            var httpResponse = await httpClient.PostAsync(funcName, content, cancellationToken);

            if (!httpResponse.IsSuccessStatusCode)
                throw new ViberClientException(
                    $"Error sending request {nameof(SetWebHook)}: {httpResponse.StatusCode}!");

            return await httpResponse.Content.ReadFromJsonAsync<TResp>(cancellationToken: cancellationToken);
        }
    }
}