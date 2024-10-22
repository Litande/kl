using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KL.Agent.API.Application.Configurations;
using KL.Agent.API.Application.Handlers;
using KL.Agent.API.Application.Services;
using KL.Agent.API.Persistent;
using KL.Agent.API.Persistent.Entities;
using KL.Agent.API.Persistent.Entities.Cache;
using KL.Agent.API.Persistent.Repositories;
using KL.Agent.API.Persistent.Repositories.Interfaces;
using KL.Nats;
using Medallion.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace KL.Agent.API.Tests;

public class AgentTestsFixture
{
    private static int _countDb;
    protected readonly IDistributedLockProvider DistributedLockProviderMock = new DistributedLockProviderMock();
    protected readonly IAgentCacheRepository AgentCacheRepository = new AgentCacheRepoTest();
    protected readonly CallInfoCacheRepoTest CallInfoCacheRepository = new();
    protected KlDbContext Context;

    private KlDbContext GetNewContext()
    {
        _countDb++;
        var options = new DbContextOptionsBuilder<KlDbContext>()
            .UseInMemoryDatabase($"kl-db-test-{_countDb}").Options;
        return new KlDbContext(options);
    }

    protected readonly IOptions<NatsPubSubOptions> PubSubjectsOptions = Options.Create(new NatsPubSubOptions
    {
        AgentReplaceResult = string.Empty
    });

    protected const long AgentId = 1;
    protected const long ClientId = 1;
    protected const string SessionId = "session-id";

    protected async Task<IAgentConnectedHandler> GetAgentConnectedHandler()
    {
        Context =  GetNewContext();
        await Context.Users.AddAsync(new User
        {
            UserId = AgentId,
            ClientId = ClientId,
            FirstName = "FirstName",
            LastName = "LastName",
            OfflineSince = DateTimeOffset.UtcNow.AddDays(-3)
        });
        await Context.SaveChangesAsync();

        var userRepo = new UserRepository(Context);
        return new AgentConnectedHandler(
            new Mock<IAgentTimeoutService>().Object,
            userRepo,
            AgentStateServiceMock
        );
    }

    protected IAgentStateService AgentStateServiceMock => new AgentStateService(
        AgentCacheRepository,
        CallInfoCacheRepository,
        DistributedLockProviderMock,
        new Mock<ICallInfoHandler>().Object,
        new Mock<IAgentCurrentStatusHandler>().Object,
        new Mock<IFeedbackTimeoutHandler>().Object,
        new Mock<INatsPublisher>().Object,
        PubSubjectsOptions,
        new Mock<ILogger<AgentStateService>>().Object,
        new Mock<IAgentStatusHistoryRepository>().Object
    );
}

internal class DistributedLockProviderMock : IDistributedLockProvider
{
    public IDistributedLock CreateLock(string name)
    {
        return new MockDistributedLock();
    }
}

internal class MockDistributedLock : IDistributedLock
{
    public string Name => throw new NotImplementedException();

    public IDistributedSynchronizationHandle Acquire(TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask<IDistributedSynchronizationHandle> AcquireAsync(TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        return new ValueTask<IDistributedSynchronizationHandle>();
    }

    public IDistributedSynchronizationHandle TryAcquire(TimeSpan timeout = default,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask<IDistributedSynchronizationHandle?> TryAcquireAsync(TimeSpan timeout = default,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

internal class AgentCacheRepoTest : IAgentCacheRepository
{
    private readonly Dictionary<long, AgentStateCache> _caches = new();

    public Task<AgentStateCache?> GetAgent(long agentId)
    {
        _caches.TryGetValue(agentId, out var agent);
        return Task.FromResult(agent);
    }

    public Task<IDictionary<long, AgentStateCache>> GetAllAgents()
    {
        throw new NotImplementedException();
    }

    public Task UpdateAgent(AgentStateCache? cache)
    {
        if (cache is null)
            return Task.CompletedTask;

        _caches[cache.AgentId] = cache;
        return Task.CompletedTask;
    }

    public string LockPrefix => null!;
}

public class CallInfoCacheRepoTest : ICallInfoCacheRepository
{
    private readonly Dictionary<string, CallInfoCache> _caches = new();

    public Task AddCallInfo(CallInfoCache callInfo)
    {
        _caches.Add(callInfo.SessionId, callInfo);
        return Task.CompletedTask;
    }

    public Task<CallInfoCache?> GetCallInfo(string sessionId)
    {
        if (string.IsNullOrEmpty(sessionId))
            return Task.FromResult<CallInfoCache?>(null);
        
        _caches.TryGetValue(sessionId, out var value);
        return Task.FromResult(value);
    }
}