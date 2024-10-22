namespace KL.RabbitMq;

internal static class DeadQueueHelper
{
    public static string GetExchangeName(string originalName)
    {
        return $"{originalName}{Constants.DeadSuffix}";
    }
    public static string GetQueueName(string originalName)
    {
        return $"{originalName}{Constants.DeadSuffix}";
    }
}
