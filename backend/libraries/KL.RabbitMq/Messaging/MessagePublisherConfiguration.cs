namespace KL.RabbitMq.Messaging;

public class MessagePublisherConfiguration
{
    public string Channel { get; set; }
    public bool Persistent { get; set; }
    public Exchange Exchange { get; set; }
    public Dictionary<string, object>? Arguments { get; set; }
}

public class Exchange
{
    public string Name { get; set; }
    public string Type { get; set; }
}

public class MessagePublisherConfiguration<T> : MessagePublisherConfiguration;
