using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;

namespace KL.RabbitMq.RetryPolicies;

internal class DefaultRabbitRetryPolicy(ILogger<IRabbitRetryPolicy> logger) : IRabbitRetryPolicy
{
    public Task Handle(ChannelAccessor channelAccessor, BasicDeliverEventArgs eventArgs, string originalQueue)
    {
        logger.LogWarning("{DefaultRabbitRetryPolicy} policy: reject. Queue: '{RoutingKey}'; {MessageId}",
            nameof(DefaultRabbitRetryPolicy), eventArgs.RoutingKey, eventArgs.BasicProperties.MessageId);

        channelAccessor.Channel.BasicNack(eventArgs.DeliveryTag, false, false);

        return Task.CompletedTask;
    }
}
