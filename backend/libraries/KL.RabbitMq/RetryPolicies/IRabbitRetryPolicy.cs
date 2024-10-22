using RabbitMQ.Client.Events;

namespace KL.RabbitMq.RetryPolicies;

internal interface IRabbitRetryPolicy
{
    Task Handle(
        ChannelAccessor channelAccessor,
        BasicDeliverEventArgs eventArgs,
        string originalQueue);
}
