﻿using Botticelli.Talks.Settings;
using Flurl;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Botticelli.Talks.OpenTts
{
    public class OpenTtsSpeaker : BaseTtsSpeaker
    {


        public OpenTtsSpeaker(IHttpClientFactory httpClientFactory,
                              ILogger<OpenTtsSpeaker> logger,
                              IOptionsMonitor<TtsSettings> settings) 
                : base(logger, httpClientFactory, settings)
        {
        }

        public override async Task<byte[]> Speak(string markedText, string voice, string lang, CancellationToken token)
        {
            var urlEncodedText = $"?voice={voice}:{lang}&text={Url.Encode(markedText)}&vocoder=high&denoiserStrength=0.03&cache=true";

            using var client = HttpClientFactory.CreateClient();

            var fullUrl = Url.Combine(Settings.CurrentValue.EngineConnection, urlEncodedText);
            var result = await client.GetAsync(fullUrl, token);

            if (!result.IsSuccessStatusCode)
            {
                Logger.LogError($"Can't get response from voice: {result.StatusCode}: {result.ReasonPhrase}!");

                return Array.Empty<byte>();
                //throw new BotticelliTalksException($"Can't get response from voice: {result.StatusCode}!");
            }

            var byteResult = await result.Content.ReadAsByteArrayAsync(token);

            byteResult = await Compress(byteResult, token);

            return byteResult;
        }

        public override async Task<byte[]> Speak(string markedText, CancellationToken token)
            => await Speak(markedText,
                           Settings.CurrentValue.DefaultVoice,
                           Settings.CurrentValue.Language,
                           token);
    }
}
