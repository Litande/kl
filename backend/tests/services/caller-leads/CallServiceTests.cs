using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KL.Caller.Leads.Models.Requests;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace KL.Caller.Leads.Tests;

public class CallServiceTests : CallServiceTestsFixture
{
    [Fact]
    public async Task TryToCallPredictive_NoAvailableBridges()
    {
        Setup_BridgeService_AnyBridgeRegistered(result: false);

        await GetCallerService().TryToCallPredictive();

        // Assert
        LeadQueueRepositoryMock
            .Verify(r => r.GetAllWithAgentsOrdered(It.IsAny<CancellationToken>()),
                Times.Never);

        VerifyPublishForCallNeverRun();

        LoggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) =>
                    string.Equals("No available bridges", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task TryToCallPredictive_NoAnyQueues()
    {
        Setup_BridgeService_AnyBridgeRegistered();
        SetupDefault_SipProviderService_GetProviderForPredictiveCall();
        Setup_LeadQueueRepository_GetAllWithAgentsOrdered();

        await GetCallerService().TryToCallPredictive();

        // Assert
        VerifyPublishForCallNeverRun();
        VerifyNotAnyLogMessage();
    }

    [Fact]
    public async Task TryToCallPredictive_NoAnyWaitingAgents()
    {
        Fill_QueuesWithAssignedAgents();

        Setup_BridgeService_AnyBridgeRegistered();
        SetupDefault_SipProviderService_GetProviderForPredictiveCall();
        Setup_LeadQueueRepository_GetAllWithAgentsOrdered();
        Setup_UserRepository_GetAgentsWithScore();
        Setup_AgentStateStore_GetWaitingAgentsByIds();

        await GetCallerService().TryToCallPredictive();

        // Assert
        AgentStateStoreMock
            .Verify(r => r.GetWaitingAgentsByIds(It.IsAny<long>(), It.IsAny<IEnumerable<long>>(), It.IsAny<CancellationToken>()),
                Times.Exactly(GetQueuesCount()));

        VerifyPublishForCallNeverRun();
    }

    [Fact]
    public async Task TryToCallPredictive_NotAnyLeads()
    {
        Fill_QueuesWithAssignedAgents(assignedAgentIds: 1);
        Fill_AgentsWithScore();
        Fill_WaitingAgents();

        Setup_BridgeService_AnyBridgeRegistered();
        SetupDefault_SipProviderService_GetProviderForPredictiveCall();
        Setup_LeadQueueRepository_GetAllWithAgentsOrdered();
        Setup_UserRepository_GetAgentsWithScore();
        Setup_AgentStateStore_GetWaitingAgentsByIds();
        Setup_GetNextLead();

        await GetCallerService().TryToCallPredictive();

        // Assert
        LeadStoreClientMock
            .Verify(r => r.GetNextLead(It.IsAny<long>(), It.IsAny<GetNextLeadRequest>(), It.IsAny<CancellationToken>()),
                Times.Exactly(GetQueuesCount()));

        VerifyPublishForCallNeverRun();
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(3, 1)]
    [InlineData(10, 2)]
    public async Task TryToCallPredictive_PublishedForCallCount(int ratio, int waitingAgentCount)
    {
        Fill_AgentsWithScore(waitingAgentCount);
        Fill_WaitingAgents(waitingAgentCount);
        Fill_QueuesWithAssignedAgents(ratio: ratio, assignedAgentIds: new long []{ 1, 2, 3, 4, 5 });
        Fill_GetNextLeadResponses(queueId: 1, 100);

        Setup_BridgeService_AnyBridgeRegistered();
        SetupDefault_SipProviderService_GetProviderForPredictiveCall();
        Setup_LeadQueueRepository_GetAllWithAgentsOrdered();
        Setup_UserRepository_GetAgentsWithScore();
        Setup_AgentStateStore_GetWaitingAgentsByIds();
        Setup_GetNextLead();
        SetupDefault_GetProductiveDialerSettingsOrDefault();
        SetupDefault_GetRtcSettings();
        Setup_PublishForCallHandler();

        await GetCallerService().TryToCallPredictive();

        // Assert
        var expectedCallToRequestsCount = waitingAgentCount * ratio;
        Assert.Equal(expectedCallToRequestsCount, GetCallToRequestCount());
    }

    [Fact]
    public async Task TryToCallPredictive_TopAgentWithTopLead()
    {
        Fill_AgentsWithScore(10, isRandomScore: true);
        Fill_WaitingAgents(10);
        Fill_QueuesWithAssignedAgents(ratio: 1, assignedAgentIds: new long[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
        Fill_GetNextLeadResponses(queueId: 1, count: 5);

        Setup_BridgeService_AnyBridgeRegistered();
        SetupDefault_SipProviderService_GetProviderForPredictiveCall();
        Setup_LeadQueueRepository_GetAllWithAgentsOrdered();
        Setup_UserRepository_GetAgentsWithScore();
        Setup_AgentStateStore_GetWaitingAgentsByIds();
        Setup_GetNextLead();
        SetupDefault_GetProductiveDialerSettingsOrDefault();
        SetupDefault_GetRtcSettings();
        Setup_PublishForCallHandler();

        await GetCallerService().TryToCallPredictive();

        // Assert
        var topAgentIdByScore = GetAgentIdsSortedByScore().First();
        var topLeadCallToRequest = GetCallToRequests().MinBy(r => r.LeadId);

        Assert.NotNull(topLeadCallToRequest);
        Assert.NotEqual(0, topAgentIdByScore);
        Assert.Equal(topAgentIdByScore, topLeadCallToRequest!.AgentId);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(6)]
    public async Task TryToCallPredictive_FixedAssigned(long assignedAgentId)
    {
        Fill_AgentsWithScore(6);
        Fill_WaitingAgents(6);
        Fill_QueuesWithAssignedAgents(ratio: 10, assignedAgentIds: new long[] { 1, 2, 3, 4, 5, 6 });
        Fill_GetNextLeadResponses(queueId: 1, count: 1, assignedAgentId);

        Setup_BridgeService_AnyBridgeRegistered();
        SetupDefault_SipProviderService_GetProviderForPredictiveCall();
        Setup_LeadQueueRepository_GetAllWithAgentsOrdered();
        Setup_UserRepository_GetAgentsWithScore();
        Setup_AgentStateStore_GetWaitingAgentsByIds();
        Setup_GetNextLead();
        SetupDefault_GetProductiveDialerSettingsOrDefault();
        SetupDefault_GetRtcSettings();
        Setup_PublishForCallHandler();

        await GetCallerService().TryToCallPredictive();

        // Assert
        LoggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, _) =>
                    o.ToString()!.Contains("assigned to lead Id")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        var callToRequest = GetCallToRequests().SingleOrDefault();

        Assert.NotNull(callToRequest);
        Assert.Equal(assignedAgentId, callToRequest!.AgentId);
    }

    [Theory]
    [InlineData(1, 3)]
    [InlineData(5, 5)]
    [InlineData(10, 7)]
    public async Task TryToCallPredictive_TopAgentCallsCount(int ratio, int leadsInQueueCount)
    {
        Fill_AgentsWithScore(6, isRandomScore: true);
        Fill_WaitingAgents(6);
        Fill_QueuesWithAssignedAgents(ratio: ratio, assignedAgentIds: new long[] { 1, 2, 3, 4, 5, 6 });
        Fill_GetNextLeadResponses(queueId: 1, count: leadsInQueueCount);

        Setup_BridgeService_AnyBridgeRegistered();
        SetupDefault_SipProviderService_GetProviderForPredictiveCall();
        Setup_LeadQueueRepository_GetAllWithAgentsOrdered();
        Setup_UserRepository_GetAgentsWithScore();
        Setup_AgentStateStore_GetWaitingAgentsByIds();
        Setup_AgentStateStore_AddCall();
        Setup_GetNextLead();
        SetupDefault_GetProductiveDialerSettingsOrDefault();
        SetupDefault_GetRtcSettings();
        Setup_PublishForCallHandler();

        await GetCallerService().TryToCallPredictive();

        // Assert
        var topAgentIdByScore = GetAgentIdsSortedByScore().First();
        var expectedAgentCallsCount = Math.Min(ratio, leadsInQueueCount);

        Assert.Equal(expectedAgentCallsCount, GetAgentCallsCount(topAgentIdByScore));
    }

    [Fact]
    public async Task TryToCallPredictive_LimitsByAgents()
    {
        Fill_AgentsWithScore(4, isRandomScore: true);
        Fill_WaitingAgents(4);
        Fill_QueuesWithAssignedAgents(ratio: 2, assignedAgentIds: new long[] { 1, 2, 3, 4 });
        Fill_GetNextLeadResponses(queueId: 1, count: 3);

        Setup_BridgeService_AnyBridgeRegistered();
        SetupDefault_SipProviderService_GetProviderForPredictiveCall();
        Setup_LeadQueueRepository_GetAllWithAgentsOrdered();
        Setup_UserRepository_GetAgentsWithScore();
        Setup_AgentStateStore_GetWaitingAgentsByIds();
        Setup_AgentStateStore_AddCall();
        Setup_GetNextLead();
        SetupDefault_GetProductiveDialerSettingsOrDefault();
        SetupDefault_GetRtcSettings();
        Setup_PublishForCallHandler();

        await GetCallerService().TryToCallPredictive();

        // Assert
        var agentIdsByScore = GetAgentIdsSortedByScore();
        Assert.Equal(2, GetAgentCallsCount(agentIdsByScore.First()));
        Assert.Equal(1, GetAgentCallsCount(agentIdsByScore[1]));
        Assert.Equal(0, GetAgentCallsCount(agentIdsByScore[2]));
        Assert.Equal(0, GetAgentCallsCount(agentIdsByScore.Last()));
    }

    [Fact]
    public async Task TryToCallPredictive_AgentsStatusBeforeCall()
    {
        Fill_AgentsWithScore(3);
        Fill_WaitingAgents(3);
        Fill_QueuesWithAssignedAgents(ratio: 2, assignedAgentIds: new long[] { 1, 2, 3 });
        Fill_GetNextLeadResponses(queueId: 1, count: 3);

        Setup_BridgeService_AnyBridgeRegistered();
        SetupDefault_SipProviderService_GetProviderForPredictiveCall();
        Setup_LeadQueueRepository_GetAllWithAgentsOrdered();
        Setup_UserRepository_GetAgentsWithScore();
        Setup_AgentStateStore_GetWaitingAgentsByIds();
        Setup_AgentStateStore_AddCall();
        Setup_GetNextLead();
        SetupDefault_GetProductiveDialerSettingsOrDefault();
        SetupDefault_GetRtcSettings();
        Setup_PublishForCallHandler();
        Setup_SaveStatusBeforeCall();
        Setup_Process_AgentsChangedStatusMessage();

        await GetCallerService().TryToCallPredictive();

        // Assert
        var expectedAgentIds = new long[] { 1, 2 };

        AgentCacheRepositoryMock.Verify(
            r => r.SaveStatusBeforeCall(expectedAgentIds),
            Times.Once);

        var expectedMessage = new AgentsChangedStatusMessage(ClientId: 1,
            new[]
            {
                new AgentChangedStatusCommand(1, AgentStatusTypes.Dialing),
                new AgentChangedStatusCommand(2, AgentStatusTypes.Dialing),
            });

        AgentsUpdateStatusHandlerMock.Verify(
            r => r.Process(It.IsAny<AgentsChangedStatusMessage>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
