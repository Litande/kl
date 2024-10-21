using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Plat4Me.DialLeadCaller.Application.Enums;
using Plat4Me.DialLeadCaller.Application.Models;
using Plat4Me.DialLeadCaller.Application.Models.Entities;
using Plat4Me.DialLeadCaller.Application.Repositories;
using Plat4Me.DialLeadCaller.Infrastructure;
using Plat4Me.DialLeadCaller.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Plat4Me.DialLeadCaller.Tests;

public class LeadQueueCacheTests
{
    private readonly IMemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());
    private readonly Mock<ILeadQueueRepository> _leadQueueRepository;
    private readonly DbContextOptions<DialDbContext> _dbContextOptions;

    private readonly List<LeadQueueAgents> _list = new()
    {
        new() { Id = 1, Ratio = 1, Name = "1", },
        new() { Id = 2, Ratio = 2, Name = "2", }
    };

    public LeadQueueCacheTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<DialDbContext>()
            .UseInMemoryDatabase(databaseName: "DialDbContextInMemory")
            .Options;

        _leadQueueRepository = new Mock<ILeadQueueRepository>();
        _leadQueueRepository
            .Setup(r => r.GetAllWithAgentsOrdered(default).Result)
            .Returns(_list);
    }

    [Fact]
    public async Task RunApplication_ShouldReturnOk_WhenApplicationStarts()
    {
        var cached1 = await LeadQueuesCached();

        ClearLeadQueuesCached(1, 10);
        _list[0].Ratio = 10;

        var cached2 = await LeadQueuesCached();
    }

    [Fact]
    public async Task LeadQueue_FutureCalls_MaxPriority_ShouldBeFirst()
    {
        await FillRandomLeadQueues();

        await using var context = new DialDbContext(_dbContextOptions);
        var repo = new LeadQueueRepository(context);
        var leadQueues = await repo.GetAllWithAgentsOrdered();

        var maxPriorityFutureCalls = leadQueues
            .Where(r => r.QueueType == LeadQueueTypes.Future)
            .MaxBy(r => r.Priority)
            ?.Priority ?? 0;

        var fist = leadQueues.First();

        Assert.Equal(LeadQueueTypes.Future, fist.QueueType);
        Assert.Equal(maxPriorityFutureCalls, fist.Priority);
    }

    private void ClearLeadQueuesCached(long queueId, long ratio)
    {
        const string cacheKey = nameof(LeadQueuesCached);

        _memoryCache.Remove(cacheKey);
    }

    private Task<IEnumerable<IGrouping<long, LeadQueueAgents>>> LeadQueuesCached(CancellationToken ct = default)
    {
        const string cacheKey = nameof(LeadQueuesCached);
        var cachedValue = _memoryCache.GetOrCreateAsync(
            cacheKey,
            async cacheEntry =>
            {
                cacheEntry.SlidingExpiration = TimeSpan.FromSeconds(3);
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(20);
                var items = await _leadQueueRepository.Object.GetAllWithAgentsOrdered(ct);
                return items.GroupBy(r => r.ClientId);
            });

        return cachedValue;
    }

    private async Task FillRandomLeadQueues(CancellationToken ct = default)
    {
        await using var context = new DialDbContext(_dbContextOptions);
        context.CallDetailRecords.RemoveRange(context.CallDetailRecords.ToArray());

        var rand = new Random();
        for (var i = 1; i <= 100; i++)
        {
            context.LeadQueues.Add(new LeadQueue
            {
                Id = i,
                QueueType = i == 50 ? LeadQueueTypes.Future : (LeadQueueTypes) rand.Next(1, 3),
                Default = rand.Next(0, 1) == 1,
                Ratio = rand.Next(1, 5),
                Priority = i == 50 ? -100 : rand.Next(1, 1000),
                Status = (LeadQueueStatusTypes)rand.Next(1, 2),
                Name = $"Name_{i}",
                AgentLeadQueues = new List<UserLeadQueue>
                {
                    new()
                    {
                        UserId = i + 10,
                        LeadQueueId = i
                    }
                }
            });
        }

        await context.SaveChangesAsync(ct);
    }
}
