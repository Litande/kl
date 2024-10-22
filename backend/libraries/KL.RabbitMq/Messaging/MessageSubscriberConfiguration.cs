namespace KL.RabbitMq.Messaging;

public class MessageSubscriberConfiguration
{
    public string Channel { get; set; }
    public RetryPolicyConfiguration? RetryPolicy { get; set; }
}

public class MessageSubscriberConfiguration<T> : MessageSubscriberConfiguration;
