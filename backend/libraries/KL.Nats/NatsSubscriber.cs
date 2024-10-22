using AlterNats;

namespace KL.Nats;

public interface INatsSubscriber
{
    Task SubscribeAsync<T>(string channel, Action<T> handler)
        where T : class;
}
public class NatsSubscriber(INatsCommand natsCommand) : INatsSubscriber
{
    public async Task SubscribeAsync<T>(string channel, Action<T> handler) where T : class
    {
        await natsCommand.SubscribeAsync(channel, handler);
    }
}
