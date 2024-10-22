using AlterNats;

namespace KL.Nats;

public interface INatsPublisher
{
    Task PublishAsync<T>(string subject, T message);
    Task PublishAsync(string subject, byte[] message);
}

public class NatsPublisher(INatsCommand natsConnection) : INatsPublisher
{
    public async Task PublishAsync<T>(string subject, T message)
    {
        try
        {
            await natsConnection.PublishAsync(subject, message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public async Task PublishAsync(string subject, byte[] message)
    {
        await natsConnection.PublishAsync(subject, message);
    }
}
