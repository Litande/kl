using RabbitMQ.Client;

namespace KL.RabbitMq;

internal class ChannelAccessor
{
    public IModel Channel { get; set; }
}
