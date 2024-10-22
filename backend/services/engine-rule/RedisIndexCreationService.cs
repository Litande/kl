using KL.Engine.Rule.Models;

namespace KL.Engine.Rule;

public class RedisIndexCreationService : IHostedService
{
    private readonly RedisConnectionProvider _provider;
    public RedisIndexCreationService(RedisConnectionProvider provider)
    {
        _provider = provider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _provider.Connection.CreateIndexAsync(typeof(LeadTrackingCache));
        await _provider.Connection.CreateIndexAsync(typeof(QueueLeadCache));
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
