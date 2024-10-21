using Plat4Me.Dial.Statistic.Api.Application.Models;
using Plat4Me.Dial.Statistic.Api.Application.Models.StatisticCache;
using Redis.OM;

namespace Plat4Me.Dial.Statistic.Api.DAL;

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
        await _provider.Connection.CreateIndexAsync(typeof(StatisticCache));
        await _provider.Connection.CreateIndexAsync(typeof(QueueLeadCache));
        await _provider.Connection.CreateIndexAsync(typeof(QueueDropRateCache));
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
