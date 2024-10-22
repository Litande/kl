namespace KL.RabbitMq.Messaging;

public interface IMessagingSubscriber<T>
{
    Task Subscribe(Func<T, Context, Task> handler);
}
