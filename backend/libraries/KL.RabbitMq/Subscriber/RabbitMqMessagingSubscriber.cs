using System.Text.Json;
using KL.RabbitMq.Messaging;
using KL.RabbitMq.RetryPolicies;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace KL.RabbitMq.Subscriber;

internal class RabbitMqMessagingSubscriber<T> : IMessagingSubscriber<T>, IDisposable
{
    private readonly ILogger<IMessagingSubscriber<T>> _logger;
    private readonly IRabbitMqPersistentConnection _connection;
    private readonly MessageSubscriberConfiguration<T> _configuration;
    private ChannelAccessor _channelAccessor;
    private Func<T, Messaging.Context, Task>? _handler;
    private readonly IRabbitRetryPolicy _retryPolicy;

    public RabbitMqMessagingSubscriber(
        ILogger<IMessagingSubscriber<T>> logger,
        IRabbitMqPersistentConnection connection,
        IOptions<MessageSubscriberConfiguration<T>> messageConfiguration,
        RetryPolicyFactory retryPolicyFactory)
    {
        _logger = logger;
        _configuration = messageConfiguration.Value;
        _connection = connection;
        _channelAccessor = new ChannelAccessor
        {
            Channel = CreateConsumerChannel()
        };

        _retryPolicy = retryPolicyFactory.Create(_configuration.RetryPolicy);
    }

    public Task Subscribe(Func<T, Messaging.Context, Task> handler)
    {
        if (_handler is not null)
        {
            _handler = handler;
            return Task.CompletedTask;
        }

        _handler = handler;

        StartBasicConsume();

        return Task.CompletedTask;
    }

    private IModel CreateConsumerChannel()
    {
        if (!_connection.IsConnected)
        {
            _connection.TryConnect();
        }

        _logger.LogTrace("Creating RabbitMQ consumer channel");

        var channel = _connection.CreateModel();
        channel.BasicQos(0, 1, false);

        channel.CallbackException += (sender, ea) =>
        {
            _logger.LogWarning(ea.Exception, "Recreating RabbitMQ consumer channel");

            _channelAccessor.Channel?.Dispose();
            _channelAccessor.Channel = CreateConsumerChannel();
            StartBasicConsume();
        };

        return channel;
    }

    private void StartBasicConsume()
    {
        _logger.LogTrace("Starting RabbitMQ basic consume to `{queue}` queue", _configuration.Channel);

        if (_channelAccessor.Channel != null)
        {
            //If the queue is not created yet will be subscribed when the queue is created
            Task.Run(() =>
            {
                var policy = Policy.Handle<BrokerUnreachableException>()
                    .Or<OperationInterruptedException>(exception => exception.ShutdownReason.ReplyCode == 404)
                    .WaitAndRetryForever(sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(1000));

                var attempt = 0;
                policy.Execute(() =>
                {
                    attempt++;
                    _logger.LogInformation("Try subscribe to `{queue}` queue, attempt={attempt}", _configuration.Channel, attempt);

                    if (_channelAccessor.Channel.IsClosed)
                    {
                        _channelAccessor.Channel.Dispose();
                        _channelAccessor.Channel = CreateConsumerChannel();
                    }

                    if (!_connection.IsConnected)
                    {
                        _connection.TryConnect();
                    }

                    var consumer = new AsyncEventingBasicConsumer(_channelAccessor.Channel);

                    consumer.Received += Consumer_Received;

                    _channelAccessor.Channel.BasicConsume(
                        queue: _configuration.Channel,
                        autoAck: false,
                        consumer: consumer);

                    _logger.LogInformation("Subscribed to `{queue}` queue successfully", _configuration.Channel);
                });
            });
        }
        else
        {
            _logger.LogError("StartBasicConsume can't call on _consumerChannel == null");
        }
    }

    private async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
    {
        var eventName = eventArgs.RoutingKey;
        _logger.LogInformation("RabbitMQ {EventName} Received", eventName);

        try
        {
            var message = JsonSerializer.Deserialize<T>(eventArgs.Body.Span);

            await _handler(message, new Messaging.Context
            {
                CorrelationId = eventArgs.BasicProperties.CorrelationId
            });

            _channelAccessor.Channel.BasicAck(eventArgs.DeliveryTag, false);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "RabbitMQ Error handle '{queue}' queue while processing  \"{Message}\"",
                _configuration.Channel, eventArgs);

            await _retryPolicy.Handle(_channelAccessor, eventArgs, _configuration.Channel);
        }
    }

    public void Dispose()
    {
        _logger.LogInformation("RabbitMQ Disposed");

        _channelAccessor.Channel?.Close();
        _connection.Dispose();
    }
}
