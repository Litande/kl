namespace KL.RabbitMq.Messaging;

public class RetryPolicyConfiguration
{
    public RetryPolicyType Type { get; set; }
    public int? MaxRetry { get; set; }
}
