using System.Net.Sockets;
using System.Text.Json;
using KL.RabbitMq.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace KL.RabbitMq.Publisher;

public interface IIntegrationEvent
{
    public string CorrelationId { get; set; }
}

public class RabbitMqMessagingPublisher<T> : IMessagingPublisher<T>, IDisposable
   // where T: IIntegrationEvent
{
    private readonly ILogger<IMessagingPublisher<T>> _logger;

    private readonly IRabbitMqPersistentConnection _connection;
    private readonly int _retryCount = 5;
    private readonly MessagePublisherConfiguration<T> _configuration;
    private readonly string _applicationName;
    private IModel _channel;
    private readonly string _eventName;


    public RabbitMqMessagingPublisher(
        IRabbitMqPersistentConnection connection,
        ILogger<IMessagingPublisher<T>> logger,
        IOptions<MessagePublisherConfiguration<T>> messageSenderConfiguration)
    {
        _connection = connection;
        _logger = logger;
        _configuration = messageSenderConfiguration.Value;

        _applicationName = AppDomain.CurrentDomain.FriendlyName;

        _channel = CreateChannel();
        _eventName = typeof(T).Name;
    }

    private IModel CreateChannel()
    {
        if (!_connection.IsConnected)
        {
            _connection.TryConnect();
        }

        _logger.LogTrace("Creating RabbitMQ channel to publish event: ({EventName})", _eventName);

        var channel = _connection.CreateModel();

        DeclareQueue(channel);

        return channel;
    }

    public virtual bool Publish(T @event, Messaging.Context? context = null)
    {
        if (!_connection.IsConnected)
        {
            _connection.TryConnect();
        }

        var policy = Policy.Handle<BrokerUnreachableException>()
            .Or<SocketException>()
            .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
            {
                _logger.LogWarning(ex, "Could not publish event: {EventName} after {Timeout}s ({ExceptionMessage})",
                    _eventName, $"{time.TotalSeconds:n1}", ex.Message);
            });

        DeclareQueue(_channel);

        var body = JsonSerializer.SerializeToUtf8Bytes(@event);

        policy.Execute(() =>
        {
            var properties = _channel.CreateBasicProperties();
            properties.CorrelationId = context?.CorrelationId ?? Guid.NewGuid().ToString();
            properties.Timestamp = new AmqpTimestamp((DateTime.UtcNow - DateTime.UnixEpoch).Seconds);
            properties.AppId = _applicationName;
            properties.Type = _eventName;
            properties.Persistent = _configuration.Persistent;
            properties.MessageId = Guid.NewGuid().ToString();

            properties.Headers = new Dictionary<string, object>();

            if (_configuration.Persistent)
            {
                properties.Headers[Constants.Rabbit_Header_DeadExchange] = DeadQueueHelper.GetExchangeName(_configuration.Exchange.Name);
                properties.Headers[Constants.Rabbit_Header_DeadQueue] = DeadQueueHelper.GetQueueName(_configuration.Channel);
            }

            properties.Headers[Constants.Header_OriginalExchange] = _configuration.Exchange.Name;
            properties.Headers[Constants.Header_OriginalQueue] = _configuration.Channel;

            if (_configuration.Persistent)
            {
                var deadExchangeName = DeadQueueHelper.GetExchangeName(_configuration.Exchange.Name);
                var deadQueueName = DeadQueueHelper.GetQueueName(_configuration.Channel);

                properties.Headers[Constants.Header_OriginalDeadExchange] = deadExchangeName;
                properties.Headers[Constants.Header_OriginalDeadQueue] = deadQueueName;
            }

            _logger.LogTrace("Publishing event to RabbitMQ: {EventName}", _eventName);

            _channel.BasicPublish(
                exchange: _configuration.Exchange.Name,
                routingKey: _configuration.Channel,
                mandatory: true,
                basicProperties: properties,
                body: body);
        });

        return true;
    }

    private void DeclareQueue(IModel channel)
    {
        channel.ExchangeDeclare(exchange: _configuration.Exchange.Name, type: _configuration.Exchange.Type);

        var queueArgs = _configuration.Arguments ?? new Dictionary<string, object>();

        if (_configuration.Persistent)
        {
            var deadExchangeName = DeadQueueHelper.GetExchangeName(_configuration.Exchange.Name);
            var deadQueueName = DeadQueueHelper.GetQueueName(_configuration.Channel);
            DeclareDeadQueue(channel, deadExchangeName, deadQueueName);

            queueArgs.TryAdd(Constants.Rabbit_Header_DeadExchange, deadExchangeName);
            queueArgs.TryAdd(Constants.Rabbit_Header_DeadQueue, deadQueueName);
        }

        channel.QueueDeclare(queue: _configuration.Channel,
            durable: _configuration.Persistent,
            exclusive: false,
            autoDelete: false,
            arguments: queueArgs);

        channel.QueueBind(queue: _configuration.Channel,
            exchange: _configuration.Exchange.Name,
            routingKey: _configuration.Channel);

        channel.CallbackException += (sender, ea) =>
        {
            _logger.LogWarning(ea.Exception, "Recreating RabbitMQ channel");

            _channel?.Dispose();
            _channel = CreateChannel();
        };
    }

    private void DeclareDeadQueue(
        IModel channel,
        string deadExchangeName,
        string deadQueueName)
    {
        channel.ExchangeDeclare(exchange: deadExchangeName, type: _configuration.Exchange.Type);

        channel.QueueDeclare(queue: deadQueueName,
            durable: _configuration.Persistent,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        channel.QueueBind(queue: deadQueueName,
            exchange: deadExchangeName,
            routingKey: deadQueueName);
    }

    public void Dispose()
    {
        _logger.LogInformation("RabbitMQ Disposed");

        _connection.Dispose();
    }
}
