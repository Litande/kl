using System.Text;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace KL.RabbitMq.RetryPolicies;

internal class RabbitRetryExponentialBackoffPolicy(ILogger<IRabbitRetryPolicy> logger, int? maxRetry)
    : IRabbitRetryPolicy
{
    private readonly int _maxRetry = maxRetry ?? 3;

    public Task Handle(
        ChannelAccessor channelAccessor,
        BasicDeliverEventArgs eventArgs,
        string originalQueue)
    {
        logger.LogWarning($"{nameof(RabbitRetryExponentialBackoffPolicy)} policy: processing. Queue: '{eventArgs.RoutingKey}'; {eventArgs.BasicProperties.MessageId}");

        eventArgs.BasicProperties.Headers.TryGetValue(Constants.Header_RetryCount, out var rawRetryCount);
        if (!int.TryParse(rawRetryCount?.ToString(), out var retryCount))
        {
            retryCount = 0;
        }

        if (retryCount >= _maxRetry)
        {
            eventArgs.BasicProperties.Headers.TryGetValue(Constants.Header_OriginalDeadExchange, out var originalDeadExchange);
            eventArgs.BasicProperties.Headers.TryGetValue(Constants.Header_OriginalDeadQueue, out var originalDeadQueue);

            if (originalDeadQueue is not null && originalDeadExchange is not null)
            {
                channelAccessor.Channel.BasicPublish(Encoding.UTF8.GetString(originalDeadExchange as byte[]), Encoding.UTF8.GetString(originalDeadQueue as byte[]), true, eventArgs.BasicProperties, eventArgs.Body);
            }

            logger.LogWarning($"{nameof(RabbitRetryExponentialBackoffPolicy)} policy: cancel. Queue: '{eventArgs.RoutingKey}'; {eventArgs.BasicProperties.MessageId}");
        }
        else
        {
            var delay = CalcBackoff(retryCount);

            var nextRetry = retryCount + 1;
            if (eventArgs.BasicProperties.Headers.ContainsKey(Constants.Header_RetryCount))
                eventArgs.BasicProperties.Headers[Constants.Header_RetryCount] = nextRetry;
            else
                eventArgs.BasicProperties.Headers.Add(Constants.Header_RetryCount, nextRetry);

            logger.LogWarning(
                $"{nameof(RabbitRetryExponentialBackoffPolicy)} policy: retrying. Queue: '{eventArgs.RoutingKey}'; {eventArgs.BasicProperties.MessageId}; retry: {retryCount}; delay: {delay}; properties: {eventArgs.BasicProperties}");

            CreateRetryQueueAndPublish(
                channelAccessor.Channel,
                eventArgs,
                originalQueue,
                delay);
        }

        channelAccessor.Channel.BasicAck(eventArgs.DeliveryTag, false);

        return Task.CompletedTask;
    }

    private void CreateRetryQueueAndPublish(
        IModel channel,
        BasicDeliverEventArgs eventArgs,
        string originalQueue,
        int delay)
    {
        var originalExchange = eventArgs.BasicProperties.Headers[Constants.Header_OriginalExchange];

        var retryExchange = $"{originalExchange}.retry";
        channel.ExchangeDeclare(retryExchange, "direct", true, false);

        var retryQueueName = $"{originalQueue}.retry.{delay}";

        channel.QueueDeclare(retryQueueName, true, false, false,
            new Dictionary<string, object>
            {
                {Constants.Rabbit_Header_DeadExchange, originalExchange},
                {Constants.Rabbit_Header_DeadQueue, originalQueue},
                {Constants.Rabbit_Header_MessageTTL, delay * 1_000},
                {Constants.Rabbit_Header_QueueExpires, delay * 1_000 * 5},
            });

        channel.QueueBind(
            queue: retryQueueName,
            exchange: retryExchange,
            routingKey: retryQueueName);

        channel.BasicPublish(retryExchange, retryQueueName, true, eventArgs.BasicProperties, eventArgs.Body);
    }

    private int CalcBackoff(int retryCount)
    {
        return (int)Math.Pow(retryCount + 1, 2);
    }
}
