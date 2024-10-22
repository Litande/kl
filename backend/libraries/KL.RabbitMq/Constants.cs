namespace KL.RabbitMq;

internal class Constants
{
    public const string DeadSuffix = ".dlx";
    public const string Header_MaxRetry = "x-max-retry";
    public const string Header_RetryCount = "x-retry-count";

    public const string Header_OriginalExchange = "x-original-letter-exchange";
    public const string Header_OriginalQueue = "x-original-letter-routing-key";

    public const string Header_OriginalDeadQueue = "x-original-dead-letter-queue";
    public const string Header_OriginalDeadExchange = "x-original-dead-letter-exchange";


    public const string Rabbit_Header_DeadExchange = "x-dead-letter-exchange";
    public const string Rabbit_Header_DeadQueue = "x-dead-letter-routing-key";
    public const string Rabbit_Header_MessageTTL = "x-message-ttl";
    public const string Rabbit_Header_QueueExpires = "x-expires";
}
