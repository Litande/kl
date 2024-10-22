namespace KL.RabbitMq.Messaging;

public interface IMessagingPublisher<in T>
{
    bool Publish(T @event, Context? context = null);
}
