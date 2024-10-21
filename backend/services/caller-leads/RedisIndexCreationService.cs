using Plat4Me.DialLeadCaller.Application.Models;
using Microsoft.Extensions.Hosting;
using Redis.OM;

namespace Plat4Me.DialLeadCaller.Infrastructure;

public class RedisIndexCreationService : IHostedService
{
    private readonly RedisConnectionProvider _provider;
    public RedisIndexCreationService(RedisConnectionProvider provider)
    {
        _provider = provider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _provider.Connection.CreateIndexAsync(typeof(CallInfoCache));
        await _provider.Connection.CreateIndexAsync(typeof(WaitingAgent));
        await _provider.Connection.CreateIndexAsync(typeof(LeadTrackingCache));
        await _provider.Connection.CreateIndexAsync(typeof(QueueLeadCache));
        await _provider.Connection.CreateIndexAsync(typeof(QueueDropRateCache));
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
