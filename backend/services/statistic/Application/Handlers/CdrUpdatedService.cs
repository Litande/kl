using Plat4Me.Dial.Statistic.Api.Application.Common;
using Plat4Me.Dial.Statistic.Api.Application.Common.Extensions;
using Plat4Me.Dial.Statistic.Api.Application.Models.Messages;
using Plat4Me.Dial.Statistic.Api.Application.Services;
using Plat4Me.Dial.Statistic.Api.DAL.Repositories;
using System.Collections.Concurrent;

namespace Plat4Me.Dial.Statistic.Api.Application.Handlers;

public class CdrUpdatedService : ICdrUpdatedService
{
    private readonly ConcurrentQueue<CdrUpdatedMessage> _updatedCdrs = new();
    private readonly ConcurrentQueue<CdrUpdatedMessage> _insertedCdrs = new();
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CdrUpdatedService> _logger;

    public CdrUpdatedService(
        ILogger<CdrUpdatedService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public void AddToQueueUpdatedCdrProcess(CdrUpdatedMessage message, CancellationToken ct = default)
    {
        _updatedCdrs.Enqueue(message);
    }

    public void AddToQueueInsertedCdrProcess(CdrUpdatedMessage message, CancellationToken ct = default)
    {
        _insertedCdrs.Enqueue(message);
    }

    public async Task Process(CancellationToken ct = default)
    {
        _logger.LogInformation("{Handler} Starting", nameof(CdrUpdatedService));

        await using var scope = _serviceProvider.CreateAsyncScope();
        var statsMemoryCacheService = scope.ServiceProvider.GetRequiredService<IStatsMemoryCacheService>();
        var clientRepository = scope.ServiceProvider.GetRequiredService<IClientRepository>();

        var clients = await clientRepository.GetAll(ct);
        foreach (var client in clients)
        {
            var cacheKey = CacheKeys.CdrKey + client.Id;
            var cacheCallDetailRecords = statsMemoryCacheService.Get(cacheKey);

            if (cacheCallDetailRecords is null)
                continue;

            if (_updatedCdrs.Count > 0)
            {
                var updatedCdrs = _updatedCdrs.DequeueChunk(_updatedCdrs.Count)
                    .Where(c => c.ClientId == client.Id).ToList();
                cacheCallDetailRecords.CallDetailRecords?.RemoveAll(x =>
                        updatedCdrs.Any(up => up.SessionId == x.SessionId));
                cacheCallDetailRecords.CallDetailRecords?.AddRange(updatedCdrs);
            }

            if (_insertedCdrs.Count > 0)
            {
                var insertedCdrs = _insertedCdrs.DequeueChunk(_insertedCdrs.Count)
                    .Where(c => c.ClientId == client.Id).ToList();
                cacheCallDetailRecords.CallDetailRecords?.AddRange(insertedCdrs);
            }

            statsMemoryCacheService.Set(cacheKey, cacheCallDetailRecords!);
        }
    }
}