using System;
using System.Threading;
using System.Threading.Tasks;
using KL.Caller.Leads.Enums;
using KL.Caller.Leads.Models.Messages;
using Moq;
using Xunit;

namespace KL.Caller.Leads.Tests;

public class AgentStatusFlowTests : AgentStatusFlowTestsFixture
{
    [Theory]
    [InlineData(CallType.Predictive, AgentStatusTypes.Dialing, AgentStatusTypes.InTheCall)]
    [InlineData(CallType.Predictive, null, AgentStatusTypes.Offline)]
    [InlineData(CallType.Manual, null, AgentStatusTypes.Offline)]
    [InlineData(CallType.Manual, AgentStatusTypes.Dialing, AgentStatusTypes.Dialing)]
    public async Task AgentAnsweredTest(
        CallType callType,
        AgentStatusTypes? currentStatus,
        AgentStatusTypes expectedStatus)
    {
        const int agentId = 1;
        var clientId = It.IsAny<long>();
        AgentTrackingCache? agentTracking = null;

        if (currentStatus.HasValue)
            agentTracking = new AgentTrackingCache(agentId, clientId, currentStatus.Value);

        await GetAgentAnsweredHandler(agentTracking).Process(new CalleeAnsweredMessage(
            clientId,
            It.IsAny<string>(),
            It.IsAny<string>(),
            callType,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow,
            agentId,
            It.IsAny<long>(),
            It.IsAny<long>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<long>()
        ));

        var agent = await AgentCacheRepository.GetAgent(agentId);
        Assert.NotNull(agent);
        Assert.Equal(expectedStatus, agent!.AgentStatus);
    }

    [Theory]
    [InlineData(CallType.Predictive, AgentStatusTypes.Dialing, AgentStatusTypes.Dialing)]
    [InlineData(CallType.Predictive, AgentStatusTypes.FillingFeedback, AgentStatusTypes.FillingFeedback)]
    [InlineData(CallType.Predictive, null, AgentStatusTypes.Offline)]
    public async Task AgentNotAnsweredTest(
        CallType callType,
        AgentStatusTypes? statusBeforeCall,
        AgentStatusTypes expectedStatus)
    {
        const string sessionId = "123";
        var agentTracking = new AgentTrackingCache(AgentId, ClientId, AgentStatusTypes.Dialing)
        {
            SessionId = sessionId,
            AgentStatusBeforeCall = statusBeforeCall
        };

        await GetAgentNotAnsweredHandler(agentTracking).Process(new AgentNotAnsweredMessage(
            ClientId,
            It.IsAny<string>(),
            sessionId,
            callType,
            AgentId,
            It.IsAny<long>(),
            It.IsAny<long>(),
            It.IsAny<bool>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<long>()));

        var agent = await AgentCacheRepository.GetAgent(AgentId);
        Assert.NotNull(agent);
        Assert.Equal(expectedStatus, agent!.AgentStatus);
    }

    [Theory]
    [InlineData(CallType.Manual, AgentStatusTypes.Dialing, AgentStatusTypes.InTheCall)]
    [InlineData(CallType.Manual, AgentStatusTypes.Offline, AgentStatusTypes.Offline)]
    [InlineData(CallType.Manual, AgentStatusTypes.InBreak, AgentStatusTypes.InBreak)]
    [InlineData(CallType.Manual, AgentStatusTypes.FillingFeedback, AgentStatusTypes.FillingFeedback)]
    [InlineData(CallType.Manual, AgentStatusTypes.WaitingForTheCall, AgentStatusTypes.WaitingForTheCall)]
    public async Task LeadAnsweredTest(
        CallType callType,
        AgentStatusTypes status,
        AgentStatusTypes expectedStatus)
    {
        await GetLeadAnsweredHandler(status, callType).Process(new CalleeAnsweredMessage(
            It.IsAny<long>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            callType,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow,
            AgentId,
            It.IsAny<long>(),
            It.IsAny<long>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<long>()
        ));

        var agent = await AgentCacheRepository.GetAgent(AgentId);
        Assert.NotNull(agent);
        Assert.Equal(expectedStatus, agent!.AgentStatus);
    }

    [Theory]
    [InlineData(CallType.Manual, null, CallFinishReasons.CallFinishedByManager,
        AgentStatusTypes.Dialing, AgentStatusTypes.Offline)]
    [InlineData(CallType.Manual, AgentStatusTypes.InBreak, CallFinishReasons.CallFinishedByManager,
        AgentStatusTypes.Dialing, AgentStatusTypes.InBreak)]
    [InlineData(CallType.Manual, null, CallFinishReasons.CallFinishedByAgent,
        AgentStatusTypes.InBreak, AgentStatusTypes.InBreak)]
    [InlineData(CallType.Manual, null, CallFinishReasons.CallFinishedByAgent,
        AgentStatusTypes.Offline, AgentStatusTypes.Offline)]
    [InlineData(CallType.Manual, null, CallFinishReasons.CallFinishedByAgent,
        AgentStatusTypes.WaitingForTheCall, AgentStatusTypes.WaitingForTheCall)]
    [InlineData(CallType.Predictive, null, CallFinishReasons.CallFinishedByAgent,
        AgentStatusTypes.InTheCall, AgentStatusTypes.FillingFeedback)]
    public async Task CallFinishedTest(
        CallType callType,
        AgentStatusTypes? beforeCallStatus,
        CallFinishReasons reason,
        AgentStatusTypes status,
        AgentStatusTypes expectedStatus)
    {
        const string sessionId = "123";

        await GetCallFinishedHandler(status, beforeCallStatus, callType, sessionId).Process(new CallFinishedMessage(
            ClientId,
            It.IsAny<string>(),
            sessionId,
            callType,
            AgentId,
            It.IsAny<long>(),
            It.IsAny<long>(),
            It.IsAny<string>(),
            reason,
            It.IsAny<string>(),
            It.IsAny<long>(),
            It.IsAny<int>(),
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow));

        var agent = await AgentCacheRepository.GetAgent(AgentId);
        Assert.NotNull(agent);
        Assert.Equal(expectedStatus, agent!.AgentStatus);
    }

    [Fact]
    public async Task ManualCallTest()
    {
        await GetManualCallHandler().Process(new ManualCallMessage(
            ClientId,
            AgentId,
            "",
            It.IsAny<string>()));

        var agent = await AgentCacheRepository.GetAgent(AgentId);
        Assert.NotNull(agent);
        Assert.Equal(AgentStatusTypes.Dialing, agent!.AgentStatus);
    }

    [Fact]
    public async Task CallAgainTest()
    {
        await GetCallAgainHandler().Process(new CallAgainMessage(
            ClientId,
            AgentId,
            string.Empty,
            It.IsAny<string>()));

        var agent = await AgentCacheRepository.GetAgent(AgentId);
        Assert.NotNull(agent);
        Assert.Equal(AgentStatusTypes.Dialing, agent!.AgentStatus);
    }

    [Theory]
    [InlineData(CallType.Predictive, AgentStatusTypes.Offline, null, AgentStatusTypes.Offline)]
    [InlineData(CallType.Predictive, AgentStatusTypes.InBreak, AgentStatusTypes.InBreak, AgentStatusTypes.InBreak)]
    [InlineData(CallType.Manual, AgentStatusTypes.FillingFeedback, AgentStatusTypes.Offline, AgentStatusTypes.Offline)]
    [InlineData(CallType.Manual, AgentStatusTypes.FillingFeedback, null, AgentStatusTypes.Offline)]
    [InlineData(CallType.Manual, AgentStatusTypes.FillingFeedback, AgentStatusTypes.InTheCall,
        AgentStatusTypes.InTheCall)]
    public async Task CallFailedTest(CallType callType, AgentStatusTypes currentStatus, AgentStatusTypes? statusBefore,
        AgentStatusTypes expectedStatus)
    {
        await GetCallFailedHandler(callType, currentStatus, statusBefore).Process(new CallFinishedMessage(
            ClientId,
            It.IsAny<string>(),
            "123",
            CallType.Manual,
            AgentId,
            It.IsAny<long>(),
            It.IsAny<long>(),
            It.IsAny<string>(),
            CallFinishReasons.Unknown,
            null,
            It.IsAny<long>(),
            It.IsAny<int>(),
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow), CancellationToken.None);

        var agent = await AgentCacheRepository.GetAgent(AgentId);
        Assert.NotNull(agent);
        Assert.Equal(expectedStatus, agent!.AgentStatus);
    }

    [Theory]
    [InlineData(CallType.Predictive, null, AgentStatusTypes.Offline)]
    [InlineData(CallType.Predictive, AgentStatusTypes.InBreak, AgentStatusTypes.InBreak)]
    [InlineData(CallType.Manual, null, AgentStatusTypes.Offline)]
    [InlineData(CallType.Manual, AgentStatusTypes.InTheCall, AgentStatusTypes.InTheCall)]
    public async Task DroppedAgentTest(CallType callType, AgentStatusTypes? statusBefore,
        AgentStatusTypes expectedStatus)
    {
        await GetDroppedAgentHandler(callType, statusBefore).Process(new DroppedAgentMessage(
            ClientId,
            "bridge-id",
            "123",
            It.IsAny<CallType>(),
            It.IsAny<long>(),
            AgentId,
            It.IsAny<long>(),
            It.IsAny<bool>(),
            It.IsAny<string>(),
            It.IsAny<long>(),
            DateTimeOffset.UtcNow,
            It.IsAny<long>(),
            It.IsAny<string>()
        ));

        var agent = await AgentCacheRepository.GetAgent(AgentId);
        Assert.NotNull(agent);
        Assert.Equal(expectedStatus, agent!.AgentStatus);
    }
}