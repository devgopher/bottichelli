using System.Runtime.CompilerServices;
using System.Text.Json;
using Botticelli.Bot.Interfaces.Client;
using Botticelli.Bus.Rabbit.Exceptions;
using Botticelli.Bus.Rabbit.Settings;
using Botticelli.Interfaces;
using Botticelli.Shared.API.Client.Requests;
using Botticelli.Shared.API.Client.Responses;
using Botticelli.Shared.Utils;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Timeout;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Botticelli.Bus.Rabbit.Client;

public class RabbitClient<TBot> : BasicFunctions<TBot>, IBusClient
    where TBot : IBot
{
    private readonly ILogger<RabbitClient<TBot>> _logger;
    private readonly IConnectionFactory _rabbitConnectionFactory;
    private readonly Dictionary<string, SendMessageResponse> _responses = new(100);
    private readonly RabbitBusSettings _settings;
    private readonly TimeSpan _timeout;
    private EventingBasicConsumer _consumer;

    public RabbitClient(IConnectionFactory rabbitConnectionFactory,
        RabbitBusSettings settings,
        ILogger<RabbitClient<TBot>> logger)
    {
        _rabbitConnectionFactory = rabbitConnectionFactory;
        _settings = settings;
        _logger = logger;
        _timeout = settings.Timeout;

        Init();
    }

    public async IAsyncEnumerable<SendMessageResponse> SendAndGetResponseSeries(SendMessageRequest request,
        [EnumeratorCancellation] CancellationToken token)
    {
        using var connection = _rabbitConnectionFactory.CreateConnection();
        using var channel = connection.CreateModel();

        Send(request, channel, GetRequestQueueName());

        if (!_responses.TryGetValue(request.Message.Uid, out var prevValue))
            yield break;

        while (prevValue.IsPartial == true && prevValue.IsFinal)
            if (_responses.TryGetValue(request.Message.Uid, out var value))
            {
                if (value.IsFinal)
                    yield return value;

                if (value.SequenceNumber > prevValue.SequenceNumber)
                    continue;

                prevValue = value;
                yield return value;
            }
    }

    public async Task<SendMessageResponse> SendAndGetResponse(SendMessageRequest request,
        CancellationToken token)
    {
        try
        {
            using var connection = _rabbitConnectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            Send(request, channel, GetRequestQueueName());

            var timeoutPolicy = Policy.TimeoutAsync<SendMessageResponse>(_timeout, TimeoutStrategy.Pessimistic);
            var resultPolicy = Policy.HandleResult<SendMessageResponse>(s => s == null)
                .WaitAndRetryAsync(int.MaxValue, _ => TimeSpan.FromMilliseconds(50));

            var combined = Policy.WrapAsync(timeoutPolicy, resultPolicy);

            var result = await combined.ExecuteAndCaptureAsync(() =>
            {
                request.Message.NotNull();
                request.Message.Uid.NotNull();
                
                return Task.FromResult(_responses.GetValueOrDefault(request.Message.Uid))!;
            });

            if (result.FinalHandledResult != default)
                throw new RabbitBusException($"Error getting a response: {result.FinalException.Message}",
                    result.FinalException);

            return result.Result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            throw;
        }
    }

    public async Task SendResponse(SendMessageResponse response, CancellationToken token)
    {
        try
        {
            using var connection = _rabbitConnectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            Send(response, channel, GetResponseQueueName());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            throw;
        }
    }

    private void Init()
    {
        var connection = _rabbitConnectionFactory.CreateConnection();
        var channel = connection.CreateModel();

        _consumer ??= new EventingBasicConsumer(channel);
        var queue = GetResponseQueueName();
        var exchange = _settings.Exchange;

        if (_settings.QueueSettings.TryCreate)
            channel.ExchangeDeclare(exchange, _settings.ExchangeType);
        else
            channel.ExchangeDeclarePassive(exchange);

        var queueDeclareResult = _settings
            .QueueSettings
            .TryCreate
            ? channel.QueueDeclare(queue, _settings.QueueSettings.Durable, false)
            : channel.QueueDeclarePassive(queue);


        channel.BasicConsume(queue, true, _consumer);

        _consumer.Received += (sender, args) =>
        {
            try
            {
                if (args?.Body == null) return;

                var response = JsonSerializer.Deserialize<SendMessageResponse>(args.Body.ToArray());

                if (response == null) return;

                response.Message.NotNull();
                response.Message.Uid.NotNull();

                _responses.Add(response.Message.Uid, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        };
    }

    private void Send(object input, IModel channel, string queue)
    {
        _ = _settings.QueueSettings is { TryCreate: true, CheckQueueOnPublish: true }
            ? channel.QueueDeclare(queue, _settings.QueueSettings.Durable, false)
            : channel.QueueDeclarePassive(queue);

        channel.QueueBind(queue, _settings.Exchange, queue);
        channel.BasicPublish(_settings.Exchange, queue, body: JsonSerializer.SerializeToUtf8Bytes(input));
    }
}