using System.Text.Json;
using Botticelli.Bot.Interfaces.Client;
using Botticelli.Bus.Rabbit.Settings;
using Botticelli.Interfaces;
using Botticelli.Shared.API.Client.Requests;
using Botticelli.Shared.API.Client.Responses;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Botticelli.Bus.Rabbit.Client;

public class RabbitEventBusClient<TBot> : BasicFunctions<TBot>, IEventBusClient
    where TBot : IBot
{
    private readonly ILogger<RabbitEventBusClient<TBot>> _logger;
    private readonly IConnectionFactory _rabbitConnectionFactory;
    private readonly RabbitBusSettings _settings;
    private EventingBasicConsumer _consumer;

    public RabbitEventBusClient(IConnectionFactory rabbitConnectionFactory,
        RabbitBusSettings settings,
        ILogger<RabbitEventBusClient<TBot>> logger)
    {
        _rabbitConnectionFactory = rabbitConnectionFactory;
        _settings = settings;
        _logger = logger;

        Init();
    }

    public event IEventBusClient.BusEventHandler OnReceived;

    public Task Send(SendMessageRequest response, CancellationToken token)
    {
        try
        {
            using var connection = _rabbitConnectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            InnerSend(response, channel, GetResponseQueueName());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            throw;
        }

        return Task.CompletedTask;
    }

    public void Dispose()
    {
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
            if (args?.Body == null) return;

            var response = JsonSerializer.Deserialize<SendMessageResponse>(args.Body.ToArray());

            if (response == null) return;

            OnReceived?.Invoke(this, response);
        };
    }

    private void InnerSend(object input, IModel channel, string queue)
    {
        _ = _settings.QueueSettings is { TryCreate: true, CheckQueueOnPublish: true }
            ? channel.QueueDeclare(queue, _settings.QueueSettings.Durable, false)
            : channel.QueueDeclarePassive(queue);

        channel.QueueBind(queue, _settings.Exchange, queue);
        channel.BasicPublish(_settings.Exchange, queue, body: JsonSerializer.SerializeToUtf8Bytes(input));
    }
}