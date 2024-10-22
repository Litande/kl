using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace KL.Caller.Leads.Tests;

public class AgentStatusFlowTestsFixture
{
    protected readonly Mock<ICDRService> CDRServiceMock = new();
    protected readonly Mock<ICDRRepository> CDRRepositoryMock = new();
    protected readonly Mock<ILeadRepository> LeadRepositoryMock = new();
    protected readonly IAgentsUpdateStatusHandler AgentsUpdateStatusHandler;
    protected readonly Mock<IQueueLeadsCacheRepository> QueueLeadsCacheRepositoryMock = new();
    protected readonly Mock<ICallerService> CallerServiceMock = new();
    protected readonly Mock<IAgentStateRepository> AgentStateRepositoryMock = new();
    protected readonly Mock<IBridgeService> BridgeServiceMock = new();
    protected readonly Mock<ISipProviderService> SipProviderServiceMock = new();
    protected readonly Mock<ISettingsService> SettingsServiceMock = new();
    protected readonly Mock<ISettingsRepository> SettingsRepositoryMock = new();
    protected readonly IDistributedLockProvider DistributedLockProviderMock = new DistributedLockProviderMock();
    protected readonly IAgentCacheRepository AgentCacheRepository = new AgentCacheRepo();
    protected readonly IAgentStatusService AgentStatusService;
    protected readonly ICallerService CallerService;

    protected readonly IOptions<PubSubjects> PubSubjectsOptions = Options.Create(new PubSubjects
    {
        AgentReplaceResult = string.Empty
    });

    protected const long AgentId = 1;
    protected const long ClientId = 1;

    public AgentStatusFlowTestsFixture()
    {
        AgentsUpdateStatusHandler = new AgentsUpdateStatusHandler(
            new Mock<INatsPublisher>().Object,
            PubSubjectsOptions,
            AgentCacheRepository,
            new Mock<ILeadCacheRepository>().Object,
            new Mock<IAgentStateRepository>().Object,
            DistributedLockProviderMock,
            new Mock<ILogger<AgentsUpdateStatusHandler>>().Object
        );

        AgentStatusService = new AgentStatusService(
            new Mock<ILogger<AgentStatusService>>().Object,
            AgentsUpdateStatusHandler,
            AgentCacheRepository,
            DistributedLockProviderMock,
            AgentStateRepositoryMock.Object
        );

        CallerService = new CallerService(
            new Mock<ILogger<CallerService>>().Object,
            new Mock<ILeadRuleEngineClient>().Object,
            AgentsUpdateStatusHandler,
            new Mock<IPublishForCallHandler>().Object,
            new Mock<ILeadQueueRepository>().Object,
            new Mock<IAgentStateRepository>().Object,
            new Mock<IUserRepository>().Object,
            new Mock<IAgentCacheRepository>().Object,
            new Mock<ILeadRepository>().Object,
            BridgeServiceMock.Object,
            SipProviderServiceMock.Object,
            SettingsServiceMock.Object
        );
    }


    protected IAgentAnsweredHandler GetAgentAnsweredHandler(AgentTrackingCache? agent)
    {
        CDRServiceMock
            .Setup(x => x.Update(It.IsAny<CalleeAnsweredMessage>(), It.IsAny<CancellationToken>()));
        LeadRepositoryMock.Setup(x => x.UpdateStatusAndGet(It.IsAny<long>(), It.IsAny<long>(),
            It.IsAny<LeadSystemStatusTypes>(), It.IsAny<LeadStatusTypes>(), It.IsAny<CancellationToken>()));
        QueueLeadsCacheRepositoryMock.Setup(x => x.UpdateStatus(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<long>(),
            It.IsAny<LeadSystemStatusTypes>(), It.IsAny<LeadStatusTypes>(), It.IsAny<CancellationToken>()));

        AgentCacheRepository.UpdateAgent(agent!);

        return new AgentAnsweredHandler(CDRServiceMock.Object,
            LeadRepositoryMock.Object,
            AgentsUpdateStatusHandler,
            new Mock<ILogger<AgentAnsweredHandler>>().Object,
            QueueLeadsCacheRepositoryMock.Object,
            AgentCacheRepository,
            DistributedLockProviderMock);
    }

    protected IAgentNotAnsweredHandler GetAgentNotAnsweredHandler(AgentTrackingCache agent)
    {
        CallerServiceMock.Setup(x => x.GetFreeAgentId(
                ClientId,
                It.IsAny<long>(),
                AgentId,
                It.IsAny<long>(),
                It.IsAny<CancellationToken>()))
            .Callback(() => { agent.AgentStatus = AgentStatusTypes.Dialing; })
            .ReturnsAsync(agent.AgentId);

        CDRServiceMock.Setup(x => x.GetBySessionId(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CallDetailRecord
            {
                CallHangupAt = null,
            });

        AgentCacheRepository.UpdateAgent(agent);

        return new AgentNotAnsweredHandler(
            new Mock<INatsPublisher>().Object,
            PubSubjectsOptions,
            CallerServiceMock.Object,
            new Mock<ILogger<AgentNotAnsweredHandler>>().Object,
            DistributedLockProviderMock,
            AgentStatusService,
            CDRServiceMock.Object
        );
    }

    protected ILeadAnsweredHandler GetLeadAnsweredHandler(AgentStatusTypes status, CallType callType)
    {
        AgentCacheRepository.UpdateAgent(new AgentTrackingCache(AgentId, ClientId, status)
        {
            CallType = callType,
        });

        return new LeadAnsweredHandler(
            CDRServiceMock.Object,
            AgentsUpdateStatusHandler,
            AgentCacheRepository,
            new Mock<ILogger<LeadAnsweredHandler>>().Object,
            DistributedLockProviderMock
        );
    }

    protected ICallFinishedHandler GetCallFinishedHandler(
        AgentStatusTypes status,
        AgentStatusTypes? beforeCallStatus,
        CallType callType,
        string sessionId)
    {
        AgentCacheRepository.UpdateAgent(new AgentTrackingCache(AgentId, It.IsAny<long>(), status)
        {
            CallType = callType,
            SessionId = sessionId,
            AgentStatusBeforeCall = beforeCallStatus
        });

        return new CallFinishedHandler(
            new Mock<ILeadRepository>().Object,
            new Mock<ILogger<CallFinishedHandler>>().Object,
            new Mock<ICDRService>().Object,
            AgentCacheRepository,
            new Mock<IQueueLeadsCacheRepository>().Object,
            new Mock<ILeadStatisticProcessing>().Object,
            new Mock<ISettingsRepository>().Object,
            AgentStatusService,
            new Mock<IAgentFilledCallInfoHandler>().Object
        );
    }

    protected IManualCallHandler GetManualCallHandler()
    {
        SipProviderServiceMock.Setup(x => x.GetProviderForManualCall(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new SipProvider());
        BridgeServiceMock.Setup(x => x.GetBridge())
            .Returns("super-bridge");
        SettingsServiceMock.Setup(x => x.GetRtcSettings(ClientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new RtcConfigurationSettings());
        SettingsServiceMock.Setup(x =>
                x.GetProductiveDialerSettingsOrDefault(ClientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new ProductiveDialerSettings());

        return new ManualCallHandler(CallerService);
    }

    protected ICallAgainHandler GetCallAgainHandler()
    {
        BridgeServiceMock.Setup(x => x.GetBridge())
            .Returns("super-bridge");
        SettingsServiceMock.Setup(x => x.GetRtcSettings(ClientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new RtcConfigurationSettings());
        SettingsServiceMock.Setup(x =>
                x.GetProductiveDialerSettingsOrDefault(ClientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new ProductiveDialerSettings());
        SipProviderServiceMock.Setup(x =>
                x.GetProviderForRecall(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new());

        CDRRepositoryMock.Setup(x => x.GetBySessionId(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new CallDetailRecord
            {
                LeadQueueId = It.IsAny<long>(),
                LeadId = It.IsAny<long>(),
                SipProviderId = It.IsAny<long>(),
                LeadPhone = string.Empty,
                CallType = CallType.Predictive,
            });
        LeadRepositoryMock.Setup(x => x.GetLeadById(ClientId, It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Lead
            {
                AssignedAgentId = It.IsAny<long>(),
            });

        return new CallAgainHandler(CallerService,
            CDRRepositoryMock.Object,
            AgentCacheRepository,
            LeadRepositoryMock.Object
        );
    }

    protected ICallFailedHandler GetCallFailedHandler(
        CallType callType,
        AgentStatusTypes currentStatus,
        AgentStatusTypes? beforeCallStatus)
    {
        AgentCacheRepository.UpdateAgent(new AgentTrackingCache(AgentId, ClientId, currentStatus)
        {
            CallType = callType,
            SessionId = "123",
            AgentStatusBeforeCall = beforeCallStatus
        });
        SettingsRepositoryMock.Setup(x =>
                x.GetValue(SettingTypes.CallFinishedReason, ClientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => JsonSerializer.Serialize(new CallFinishedSettings()));

        return new CallFailedHandler(
            new Mock<ILeadRepository>().Object,
            new Mock<INatsPublisher>().Object,
            PubSubjectsOptions,
            new Mock<ILogger<CallFailedHandler>>().Object,
            new Mock<ICDRService>().Object,
            SettingsRepositoryMock.Object,
            new Mock<IQueueLeadsCacheRepository>().Object,
            DistributedLockProviderMock,
            new Mock<ILeadStatisticProcessing>().Object,
            AgentStatusService
        );
    }

    protected IDroppedAgentHandler GetDroppedAgentHandler(CallType callType, AgentStatusTypes? beforeCallStatus)
    {
        AgentCacheRepository.UpdateAgent(new AgentTrackingCache(AgentId, It.IsAny<long>(), AgentStatusTypes.InTheCall)
        {
            CallType = callType,
            SessionId = "123",
            AgentStatusBeforeCall = beforeCallStatus
        });

        SettingsRepositoryMock.Setup(x =>
                x.GetValue(SettingTypes.CallFinishedReason, It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => JsonSerializer.Serialize(new CallFinishedSettings()));


        return new DroppedAgentHandler(
            new Mock<ICDRService>().Object,
            AgentStatusService
        );
    }
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

internal class AgentCacheRepo : IAgentCacheRepository
{
    private readonly Dictionary<long, AgentTrackingCache> _caches = new();

    public Task<AgentTrackingCache?> GetAgent(long agentId)
    {
        _caches.TryGetValue(agentId, out var agent);
        return Task.FromResult(agent);
    }

    public Task<IEnumerable<AgentTrackingCache>> GetAllAgents()
    {
        throw new NotImplementedException();
    }

    public Task UpdateAgent(AgentTrackingCache? cache)
    {
        if (cache is null)
            return Task.CompletedTask;

        _caches[cache.AgentId] = cache;
        return Task.CompletedTask;
    }

    public Task SaveStatusBeforeCall(IEnumerable<long> agentIds)
    {
        throw new NotImplementedException();
    }

    public Task SaveStatusBeforeCall(long agentId)
    {
        throw new NotImplementedException();
    }

    public async Task IncreaseCallAgain(long agentId)
    {
        _caches.TryGetValue(agentId, out var agentCache);

        if (agentCache is not null)
        {
            agentCache.CallAgainCount = agentCache.CallAgainCount.HasValue
                ? agentCache.CallAgainCount + 1
                : 1;

            await UpdateAgent(agentCache);
        }
    }

    public string LockPrefix => null!;
}