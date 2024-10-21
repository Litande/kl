using Microsoft.Extensions.Logging;
using Moq;
using Plat4Me.DialLeadCaller.Application.Clients;
using Plat4Me.DialLeadCaller.Application.Enums;
using Plat4Me.DialLeadCaller.Application.Handlers.Contracts;
using Plat4Me.DialLeadCaller.Application.Models;
using Plat4Me.DialLeadCaller.Application.Models.Entities;
using Plat4Me.DialLeadCaller.Application.Models.Entities.Settings;
using Plat4Me.DialLeadCaller.Application.Models.Messages;
using Plat4Me.DialLeadCaller.Application.Models.Requests;
using Plat4Me.DialLeadCaller.Application.Models.Responses;
using Plat4Me.DialLeadCaller.Application.Repositories;
using Plat4Me.DialLeadCaller.Application.Services;
using Plat4Me.DialLeadCaller.Application.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Plat4Me.DialLeadCaller.Tests;

public class CallServiceTestsFixture
{
    protected readonly Mock<ILogger<CallerService>> LoggerMock = new();
    protected readonly Mock<ILeadRuleEngineClient> LeadStoreClientMock = new();
    protected readonly Mock<IAgentsUpdateStatusHandler> AgentsUpdateStatusHandlerMock = new();
    protected readonly Mock<IPublishForCallHandler> PublishForCallHandlerMock = new();
    protected readonly Mock<ILeadQueueRepository> LeadQueueRepositoryMock = new();
    protected readonly Mock<IAgentStateRepository> AgentStateStoreMock = new();
    protected readonly Mock<IUserRepository> UserRepositoryMock = new();
    protected readonly Mock<IAgentCacheRepository> AgentCacheRepositoryMock = new();
    protected readonly Mock<ILeadRepository> LeadRepositoryMock = new();
    protected readonly Mock<IBridgeService> BridgeServiceMock = new();
    protected readonly Mock<ISettingsService> SettingsServiceMock = new();
    protected readonly Mock<ISipProviderService> SipProviderServiceMock = new();



    // Counts
    protected int GetQueuesCount() => _queueWithAssignedAgentsList.Count;
    protected int GetCallToRequestCount() => _callToRequestPublishList.Count;
    protected long[] GetAgentIdsSortedByScore() => _agentsWithScore
        .OrderByDescending(r => r.Value.Score)
        .Select(r => r.Value.AgentId)
        .ToArray();
    protected IReadOnlyCollection<CallToRequest> GetCallToRequests() => _callToRequestPublishList;
    protected int GetAgentCallsCount(long agentId) => _agentIdCalls.Count(r => r == agentId);





    // Data
    private const long ClientId = 1;
    private const string BridgeId = "test-bridge-id-1";
    private const string IceServer = "test-ice-server-1";

    private readonly List<LeadQueueAgents> _queueWithAssignedAgentsList = new();

    private readonly RtcConfigurationSettings _rtcSettings = new()
    {
        IceServers = new[] { IceServer }
    };

    private readonly ProductiveDialerSettings _productiveDialerSettings = new()
    {
        MaxCallDuration = 10,
        RingingTimeout = 5,
    };

    private List<WaitingAgent> _waitingAgents = new();
    private Dictionary<long, AgentScore> _agentsWithScore = new();
    private Queue<GetNextLeadResponse> _getNextLeadResponses = new();

    private SipProvider _sipProvider = new()
    {
        Id = 1,
        ProviderName = "test-sip-provider-name-1",
        ProviderUserName = "test-sip-provider-username-1",
        ProviderAddress = "test-sip-provider-address-1",
        Status = SipProviderStatus.Enable,
    };





    // Fills data
    protected void Fill_AgentsWithScore(int count = 1, bool isRandomScore = false)
    {
        var rnd = new Random();
        for (var i = 1; i <= count; i++)
        {
            var score = isRandomScore
                ? rnd.Next(100)
                : count - i + 1;

            _agentsWithScore.Add(i, new AgentScore(AgentId: i, Score: score));
        }
    }

    protected void Fill_WaitingAgents(int count = 1)
    {
        for (var i = 1; i <= count; i++)
        {
            _waitingAgents.Add(new WaitingAgent(ClientId, agentId: i, AgentStatusTypes.WaitingForTheCall));
        }
    }

    protected void Fill_GetNextLeadResponses(
        long queueId,
        int count = 1,
        long? assignedAgentId = null)
    {
        for (var i = 1; i <= count; i++)
        {
            var item = new GetNextLeadResponse(
                LeadQueueId: queueId,
                LeadId: i,
                LeadPhone: "test-lead-phone-" + i,
                AssignedAgentId: assignedAgentId,
                IsTest: false);

            _getNextLeadResponses.Enqueue(item);
        }
    }

    protected void Fill_QueuesWithAssignedAgents(
        LeadQueueTypes queueType = LeadQueueTypes.Default,
        double ratio = 1,
        params long[] assignedAgentIds)
    {
        var last = _queueWithAssignedAgentsList.MaxBy(r => r.Id);
        var id = (last?.Id ?? 0) + 1;

        _queueWithAssignedAgentsList.Add(new LeadQueueAgents
        {
            ClientId = ClientId,
            Id = id,
            QueueType = queueType,
            Ratio = ratio,
            Priority = 10,
            Name = "test-queue-" + id,
            AssignedAgentIds = assignedAgentIds
        });
    }

    protected void Fill_Default_QueuesWithAssignedAgents()
    {
        Fill_QueuesWithAssignedAgents(LeadQueueTypes.Future, 1D, 1, 2, 3);
        Fill_QueuesWithAssignedAgents(LeadQueueTypes.Default, 2D, 2, 3);
        Fill_QueuesWithAssignedAgents(LeadQueueTypes.Default, 3D, 3, 4);
    }

    private GetNextLeadResponse? TryGetNextLeadResponse()
    {
        _getNextLeadResponses.TryDequeue(out var r);
        return r;
    }





    // Setups
    protected void Setup_BridgeService_AnyBridgeRegistered(bool result = true)
    {
        BridgeServiceMock
            .Setup(r => r.AnyBridgeRegistered())
            .Returns(result);

        var bridgeIdResult = result
            ? BridgeId
            : null;

        BridgeServiceMock
            .Setup(r => r.GetBridge())
            .Returns(bridgeIdResult);
    }

    protected void Setup_SipProviderService_GetProviderForPredictiveCall(SipProvider? result)
    {
        SipProviderServiceMock
            .Setup(r => r.GetProviderForPredictiveCall(It.IsAny<CancellationToken>()).Result)
            .Returns(result);
    }

    protected void Setup_LeadQueueRepository_GetAllWithAgentsOrdered()
    {
        LeadQueueRepositoryMock
            .Setup(r => r.GetAllWithAgentsOrdered(It.IsAny<CancellationToken>()).Result)
            .Returns(_queueWithAssignedAgentsList);
    }

    private void Setup_GetRtcSettings(IDictionary<long, RtcConfigurationSettings?> results)
    {
        SettingsServiceMock
            .Setup(r => r.GetRtcSettings(It.IsAny<IReadOnlyCollection<long>>(), It.IsAny<CancellationToken>()).Result)
            .Returns(results);
    }

    private void Setup_GetProductiveDialerSettingsOrDefault(IDictionary<long, ProductiveDialerSettings> results)
    {
        SettingsServiceMock
            .Setup(r => r.GetProductiveDialerSettingsOrDefault(It.IsAny<IReadOnlyCollection<long>>(), It.IsAny<CancellationToken>()).Result)
            .Returns(results);
    }

    protected void Setup_UserRepository_GetAgentsWithScore()
    {
        UserRepositoryMock
            .Setup(r => r.GetAgentsWithScore(ClientId, It.IsAny<long[]>(), It.IsAny<CancellationToken>()).Result)
            .Returns(_agentsWithScore);
    }

    protected void Setup_AgentStateStore_GetWaitingAgentsByIds()
    {
        AgentStateStoreMock
            .Setup(r => r.GetWaitingAgentsByIds(ClientId, It.IsAny<long[]>(), It.IsAny<CancellationToken>()).Result)
            .Returns(_waitingAgents);
    }

    private List<long> _agentIdCalls = new();
    protected void Setup_AgentStateStore_AddCall()
    {
        AgentStateStoreMock
            .Setup(r => r.AddCall(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()).Result)
            .Callback((long agentId, string _, CancellationToken _) => _agentIdCalls.Add(agentId))
            .Returns(_agentIdCalls.Count);
    }

    private List<GetNextLeadRequest> _getNextLeadRequestList = new();
    protected void Setup_GetNextLead()
    {
        LeadStoreClientMock
            .Setup(r => r.GetNextLead(ClientId, It.IsAny<GetNextLeadRequest>(), It.IsAny<CancellationToken>()).Result)
            .Callback((long _, GetNextLeadRequest request, CancellationToken _) =>
                _getNextLeadRequestList.Add(request))
            .Returns(TryGetNextLeadResponse);
    }

    private List<CallToRequest> _callToRequestPublishList = new();
    protected void Setup_PublishForCallHandler()
    {
        PublishForCallHandlerMock
            .Setup(r => r.Process(It.IsAny<IEnumerable<CallToRequest>>(), It.IsAny<CancellationToken>()))
            .Callback((IEnumerable<CallToRequest> requests, CancellationToken _) =>
                _callToRequestPublishList.AddRange(requests))
            .Returns(Task.CompletedTask);
    }

    protected void Setup_SaveStatusBeforeCall()
    {
        AgentCacheRepositoryMock
            .Setup(r => r.SaveStatusBeforeCall(It.IsAny<IEnumerable<long>>()))
            .Returns(Task.CompletedTask);
    }

    private List<AgentsChangedStatusMessage> _agentChangedStatusMessageList = new();
    protected void Setup_Process_AgentsChangedStatusMessage()
    {
        AgentsUpdateStatusHandlerMock
            .Setup(r => r.Process(It.IsAny<AgentsChangedStatusMessage>(), It.IsAny<CancellationToken>()))
            .Callback((AgentsChangedStatusMessage message, CancellationToken _) =>
                _agentChangedStatusMessageList.Add(message))
            .Returns(Task.CompletedTask);
    }





    // Default setups
    protected void SetupDefault_GetRtcSettings() =>
        Setup_GetRtcSettings(new Dictionary<long, RtcConfigurationSettings?> { { ClientId, _rtcSettings } });

    protected void SetupDefault_GetProductiveDialerSettingsOrDefault() =>
        Setup_GetProductiveDialerSettingsOrDefault(new Dictionary<long, ProductiveDialerSettings> { { ClientId, _productiveDialerSettings } });

    protected void SetupDefault_SipProviderService_GetProviderForPredictiveCall() =>
        Setup_SipProviderService_GetProviderForPredictiveCall(_sipProvider);





    // Verifies
    protected void VerifyNotAnyLogMessage() => LoggerMock.Verify(
        x => x.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((o, t) => true),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
        Times.Never);

    protected void VerifyPublishForCallNeverRun() => PublishForCallHandlerMock
        .Verify(r => r.Process(It.IsAny<IEnumerable<CallToRequest>>(), It.IsAny<CancellationToken>()),
            Times.Never);





    // Services
    protected ICallerService GetCallerService() =>
        new CallerService(
            LoggerMock.Object,
            LeadStoreClientMock.Object,
            AgentsUpdateStatusHandlerMock.Object,
            PublishForCallHandlerMock.Object,
            LeadQueueRepositoryMock.Object,
            AgentStateStoreMock.Object,
            UserRepositoryMock.Object,
            AgentCacheRepositoryMock.Object,
            LeadRepositoryMock.Object,
            BridgeServiceMock.Object,
            SipProviderServiceMock.Object,
            SettingsServiceMock.Object);
}
