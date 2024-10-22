using KL.Manager.API.Persistent.Entities.Cache;
using Redis.OM;

namespace KL.Manager.API.Workers;

public class RedisIndexCreationService : IHostedService
{
    private readonly RedisConnectionProvider _provider;
    public RedisIndexCreationService(RedisConnectionProvider provider)
    {
        _provider = provider;
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _provider.Connection.CreateIndexAsync(typeof(AgentStateCache));
        await _provider.Connection.CreateIndexAsync(typeof(CallInfoCache));
        await _provider.Connection.CreateIndexAsync(typeof(LeadTrackingCache));
        await _provider.Connection.CreateIndexAsync(typeof(StatisticCache));
        await _provider.Connection.CreateIndexAsync(typeof(QueueLeadCache));
        await _provider.Connection.CreateIndexAsync(typeof(QueueDropRateCache));
        await _provider.Connection.CreateIndexAsync(typeof(BlockedUserCache));
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
