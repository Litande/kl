using System;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace KL.Agent.API.Tests;

public class AgentTests : AgentTestsFixture
{
    [Fact]
    public async Task AgentConnected_CallNotFinished_ShouldBe_InTheCall_Test()
    {
        var agentCache = new AgentStateCache(AgentId, ClientId, AgentInternalStatusTypes.Offline)
        {
            CallSession = SessionId
        };
        await AgentCacheRepository.UpdateAgent(agentCache);

        var callSession = new CallInfoCache(SessionId, AgentId, ClientId)
        {
            CallType = CallType.Predictive,
            AgentAnsweredAt = DateTimeOffset.UtcNow.AddMinutes(-5).ToUnixTimeSeconds(),
        };
        await CallInfoCacheRepository.AddCallInfo(callSession);
        
        await AgentStateServiceMock.AgentConnected(AgentId, ClientId);

        agentCache = await AgentCacheRepository.GetAgent(AgentId);
        
        Assert.Equal(AgentStatusTypes.InTheCall, agentCache!.AgentDisplayStatus);
    }
    
    
    [Fact]
    public async Task AgentConnected_AgentNotAnswered_ShouldBe_Dialing_Test()
    {
        var agentCache = new AgentStateCache(AgentId, ClientId, AgentInternalStatusTypes.Offline) 
        {
            CallSession = SessionId
        };
        await AgentCacheRepository.UpdateAgent(agentCache);

        var callSession = new CallInfoCache(SessionId, AgentId, ClientId)
        {
            CallType = CallType.Predictive,
        };
        await CallInfoCacheRepository.AddCallInfo(callSession);
        
        await AgentStateServiceMock.AgentConnected(AgentId, ClientId);
        
        agentCache = await AgentCacheRepository.GetAgent(AgentId);

        Assert.Equal(AgentStatusTypes.Dialing, agentCache!.AgentDisplayStatus);
    }
    
    [Fact]
    public async Task AgentConnected_CallFinished_ShouldBe_FillingFeedback_Test()
    {
        var agentCache = new AgentStateCache(AgentId, ClientId, AgentInternalStatusTypes.Offline)
        {
            CallSession = SessionId
        };
        await AgentCacheRepository.UpdateAgent(agentCache);

        var callSession = new CallInfoCache(SessionId, AgentId, ClientId)
        {
            CallType = CallType.Predictive,
            AgentAnsweredAt = DateTimeOffset.UtcNow.AddMinutes(-5).ToUnixTimeSeconds(),
            CallFinishedAt = DateTimeOffset.UtcNow.AddMinutes(-1).ToUnixTimeSeconds(),
        };
        await CallInfoCacheRepository.AddCallInfo(callSession);
        
        await AgentStateServiceMock.AgentConnected(AgentId, ClientId);
        
        agentCache = await AgentCacheRepository.GetAgent(AgentId);

        Assert.Equal(AgentStatusTypes.FillingFeedback, agentCache!.AgentDisplayStatus);
    }
    
    [Fact]
    public async Task AgentConnected_NoCall_ShouldBe_Offline_Test()
    {
        var agentCache = new AgentStateCache(AgentId, ClientId, AgentInternalStatusTypes.Online);
        await AgentCacheRepository.UpdateAgent(agentCache);
        
        await AgentStateServiceMock.AgentConnected(AgentId, ClientId);
        
        agentCache = await AgentCacheRepository.GetAgent(AgentId);

        Assert.Equal(AgentStatusTypes.Offline, agentCache!.AgentDisplayStatus);
    }
    
    [Fact]
    public async Task AgentDisconnected_NoCall_ShouldBe_Offline_Test()
    {
        var agentCache = new AgentStateCache(AgentId, ClientId, AgentInternalStatusTypes.Online);
        await AgentCacheRepository.UpdateAgent(agentCache);
        
        await AgentStateServiceMock.AgentConnected(AgentId, ClientId);
        
        agentCache = await AgentCacheRepository.GetAgent(AgentId);

        Assert.Equal(AgentStatusTypes.Offline, agentCache!.AgentDisplayStatus);
    }

    [Theory]
    [InlineData(AgentInternalStatusTypes.WaitingForTheCall, AgentStatusTypes.WaitingForTheCall, 12, AgentStatusTypes.WaitingForTheCall)]
    [InlineData(AgentInternalStatusTypes.WaitingForTheCall, AgentStatusTypes.WaitingForTheCall, 0, AgentStatusTypes.Dialing)]
    [InlineData(AgentInternalStatusTypes.Offline, AgentStatusTypes.Offline, 1, AgentStatusTypes.Offline)]
    [InlineData(AgentInternalStatusTypes.InBreak, AgentStatusTypes.InBreak, 2, AgentStatusTypes.InBreak)]
    public async Task CallInitiatedTest(
        AgentInternalStatusTypes status,
        AgentStatusTypes secondStatus,
        long assignedCallCount,
        AgentStatusTypes expected)
    {
        var agentCache = new AgentStateCache(AgentId, ClientId, status)
        {
            AssignedCallCount = assignedCallCount,
            AgentDisplayStatus = secondStatus,
        };

        await AgentCacheRepository.UpdateAgent(agentCache);
        await AgentStateServiceMock.Handle(new CallInitiatedMessage(
            ClientId,
            It.IsAny<string>(),
            SessionId,
            CallType.Manual,
            AgentId,
            It.IsAny<long?>(),
            It.IsAny<long?>(),
            It.IsAny<string>()
        ));

        var agent = await AgentCacheRepository.GetAgent(AgentId);

        Assert.Equal(expected, agent!.AgentDisplayStatus);
    }

    [Theory]
    [InlineData(AgentInternalStatusTypes.WaitingForTheCall, AgentStatusTypes.WaitingForTheCall, 12,
        AgentStatusTypes.Dialing)]
    [InlineData(AgentInternalStatusTypes.WaitingForTheCall, AgentStatusTypes.WaitingForTheCall, 1,
        AgentStatusTypes.WaitingForTheCall)]
    [InlineData(AgentInternalStatusTypes.Offline, AgentStatusTypes.Offline, 1, AgentStatusTypes.Offline)]
    [InlineData(AgentInternalStatusTypes.InBreak, AgentStatusTypes.InBreak, 1, AgentStatusTypes.InBreak)]
    public async Task AgentReplacedTest(
        AgentInternalStatusTypes status,
        AgentStatusTypes secondStatus,
        long assignedCallCount,
        AgentStatusTypes expected)
    {
        var agentCache = new AgentStateCache(AgentId, ClientId, status)
        {
            AssignedCallCount = assignedCallCount,
            AgentDisplayStatus = secondStatus,
        };

        await AgentCacheRepository.UpdateAgent(agentCache);
        await AgentStateServiceMock.Handle(new AgentReplacedMessage(
            ClientId,
            AgentId,
            SessionId,
            It.IsAny<long>()
        ));

        var agent = await AgentCacheRepository.GetAgent(AgentId);

        Assert.Equal(expected, agent!.AgentDisplayStatus);
    }

    [Theory]
    [InlineData(CallType.Predictive, AgentStatusTypes.Dialing, null, AgentStatusTypes.Dialing)]
    [InlineData(CallType.Predictive, AgentStatusTypes.Dialing, 50, AgentStatusTypes.InTheCall)]
    [InlineData(CallType.Manual, AgentStatusTypes.Dialing, 50, AgentStatusTypes.InTheCall)]
    [InlineData(CallType.Manual, AgentStatusTypes.Dialing, null, AgentStatusTypes.Dialing)]
    public async Task AnsweredTest(
        CallType callType,
        AgentStatusTypes currentStatus,
        long? answeredMinutesAgo,
        AgentStatusTypes expectedStatus)
    {
        var agentTracking = new AgentStateCache(AgentId, ClientId, AgentInternalStatusTypes.WaitingForTheCall)
        {
            AgentDisplayStatus = currentStatus
        };

        DateTimeOffset? answeredAt = answeredMinutesAgo.HasValue
            ? DateTimeOffset.UtcNow.AddMinutes(-answeredMinutesAgo.Value)
            : null;

        await AgentCacheRepository.UpdateAgent(agentTracking);
        await AgentStateServiceMock.Handle(new CalleeAnsweredMessage(
            ClientId,
            It.IsAny<string>(),
            It.IsAny<string>(),
            callType,
            answeredAt,
            answeredAt,
            DateTimeOffset.UtcNow.AddDays(-1),
            AgentId,
            It.IsAny<long>(),
            It.IsAny<long>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<long>()
        ));

        var agent = await AgentCacheRepository.GetAgent(AgentId);
        Assert.Equal(expectedStatus, agent!.AgentDisplayStatus);
    }

    [Theory]
    [InlineData(CallType.Manual, null, AgentStatusTypes.Offline)]
    [InlineData(CallType.Predictive, null, AgentStatusTypes.Offline)]
    [InlineData(CallType.Predictive, 5, AgentStatusTypes.FillingFeedback)]
    public async Task CallFinishedTest(
        CallType callType,
        long? assignedCallCount,
        AgentStatusTypes expectedStatus)
    {
        var agentCache = new AgentStateCache(AgentId, ClientId, AgentInternalStatusTypes.Online)
        {
            AgentDisplayStatus = AgentStatusTypes.InTheCall,
            CallSession = SessionId,
            AssignedCallCount = assignedCallCount.GetValueOrDefault(),
        };

        await AgentCacheRepository.UpdateAgent(agentCache);
        await AgentStateServiceMock.Handle(new CallFinishedMessage(
            ClientId,
            It.IsAny<string>(),
            SessionId,
            callType,
            AgentId,
            It.IsAny<long>(),
            It.IsAny<long>(),
            It.IsAny<string>(),
            It.IsAny<CallFinishReasons>(),
            It.IsAny<string>(),
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow,
            "",
            ""));

        var agent = await AgentCacheRepository.GetAgent(AgentId);
        Assert.NotNull(agent);
        Assert.Equal(expectedStatus, agent!.AgentDisplayStatus);
    }


    [Theory]
    [InlineData(CallType.Manual, AgentStatusTypes.InTheCall, AgentInternalStatusTypes.Online, null,
        AgentStatusTypes.Offline)]
    [InlineData(CallType.Manual, AgentStatusTypes.InTheCall, AgentInternalStatusTypes.Online, 5,
        AgentStatusTypes.Dialing)]
    [InlineData(CallType.Predictive, AgentStatusTypes.InTheCall, AgentInternalStatusTypes.Online, 5,
        AgentStatusTypes.FillingFeedback)]
    [InlineData(CallType.Predictive, AgentStatusTypes.InTheCall, AgentInternalStatusTypes.Online, null,
        AgentStatusTypes.Offline)]
    public async Task CallFailedTest(
        CallType callType,
        AgentStatusTypes currentStatus,
        AgentInternalStatusTypes secondStatus,
        long? assignedCallCount,
        AgentStatusTypes expectedStatus)
    {
        var agentCache = new AgentStateCache(AgentId, ClientId, secondStatus)
        {
            CallSession = SessionId,
            AgentDisplayStatus = currentStatus,
            AssignedCallCount = assignedCallCount.GetValueOrDefault()
        };

        await AgentCacheRepository.UpdateAgent(agentCache);
        await AgentStateServiceMock.Handle(new CallFinishedMessage(
            ClientId,
            It.IsAny<string>(),
            SessionId,
            callType,
            AgentId,
            It.IsAny<long>(),
            It.IsAny<long>(),
            It.IsAny<string>(),
            CallFinishReasons.Unknown,
            null,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow));

        var agent = await AgentCacheRepository.GetAgent(AgentId);

        Assert.NotNull(agent);
        Assert.Equal(expectedStatus, agent!.AgentDisplayStatus);
    }

    [Theory]
    [InlineData(AgentInternalStatusTypes.Offline, null, AgentStatusTypes.Offline)]
    [InlineData(AgentInternalStatusTypes.Online, 5, AgentStatusTypes.Dialing)]
    [InlineData(AgentInternalStatusTypes.Online, null, AgentStatusTypes.Offline)]
    [InlineData(AgentInternalStatusTypes.WaitingForTheCall, null, AgentStatusTypes.WaitingForTheCall)]
    [InlineData(AgentInternalStatusTypes.InBreak, null, AgentStatusTypes.InBreak)]
    public async Task DroppedAgentTest(AgentInternalStatusTypes status, long? countAssigned,
        AgentStatusTypes expectedStatus)
    {
        var agentCache = new AgentStateCache(AgentId, ClientId, status)
        {
            AssignedCallCount = countAssigned.GetValueOrDefault()
        };

        await AgentCacheRepository.UpdateAgent(agentCache);
        await AgentStateServiceMock.Handle(new DroppedAgentMessage(
            ClientId,
            It.IsAny<string>(),
            SessionId,
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
        Assert.Equal(expectedStatus, agent!.AgentDisplayStatus);
    }

    [Theory]
    [InlineData(AgentInternalStatusTypes.Online, AgentStatusTypes.Offline)]
    [InlineData(null, AgentStatusTypes.Offline)]
    [InlineData(AgentInternalStatusTypes.WaitingForTheCall, AgentStatusTypes.Offline)]
    public async Task FeedBackFilledTest(AgentInternalStatusTypes? status, AgentStatusTypes expected)
    {
        if (status.HasValue)
            await AgentCacheRepository.UpdateAgent(new AgentStateCache(AgentId, ClientId, status.Value));

        await AgentStateServiceMock.Handle(new LeadFeedbackFilledMessage(
            ClientId,
            AgentId,
            null,
            SessionId,
            It.IsAny<long>(),
            It.IsAny<LeadStatusTypes>(),
            null)
        );


        var agent = await AgentCacheRepository.GetAgent(AgentId);
        Assert.Equal(expected, agent!.AgentDisplayStatus);
    }
}