using Plat4Me.DialRuleEngine.Application.Models;
using Microsoft.Extensions.Hosting;
using Redis.OM;

namespace Plat4Me.DialRuleEngine.Infrastructure;

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
