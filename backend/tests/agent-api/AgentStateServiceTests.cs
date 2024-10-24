﻿using System.Threading;
using System.Threading.Tasks;
using KL.Agent.API.Application.Enums;
using KL.Agent.API.Persistent.Entities.Cache;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace KL.Agent.API.Tests;

public class AgentStateServiceTests : AgentTestsFixture
{
    [Fact]
    public async Task AgentOfflineSinceTest()
    {
        await (await GetAgentConnectedHandler()).Handle(ClientId, AgentId);
        var user = await Context.Users.FirstAsync(x => x.UserId == AgentId);

        Assert.Null(user.OfflineSince);
    }

    [Fact]
    public async Task AgentHasCall_CannotStartManualCall()
    {
        await AgentCacheRepository.UpdateAgent(new AgentStateCache(AgentId, ClientId, AgentInternalStatusTypes.Online)
        {
            CallSession = SessionId,
        });
        var canStartCall = await AgentStateServiceMock
            .CanStartManualCall(AgentId, ClientId, It.IsAny<CancellationToken>());

        Assert.False(canStartCall);
    }

    [Fact]
    public async Task AgentHasNotCall_StatusWaitingCall_CannotStartManualCall()
    {
        await AgentCacheRepository.UpdateAgent(new AgentStateCache(AgentId, ClientId,
            AgentInternalStatusTypes.WaitingForTheCall));
        var canStartCall = await AgentStateServiceMock
            .CanStartManualCall(AgentId, ClientId, It.IsAny<CancellationToken>());

        Assert.False(canStartCall);
    }

    [Fact]
    public async Task AgentHasNotCall_StatusInBreak_CanStartManualCall()
    {
        await AgentCacheRepository.UpdateAgent(new AgentStateCache(AgentId, ClientId,
            AgentInternalStatusTypes.InBreak));
        var canStartCall = await AgentStateServiceMock
            .CanStartManualCall(AgentId, ClientId, It.IsAny<CancellationToken>());

        Assert.True(canStartCall);
    }

    [Fact]
    public async Task AgentHasNot_CannotStartManualCall()
    {
        var canStartCall = await AgentStateServiceMock
            .CanStartManualCall(AgentId, ClientId, It.IsAny<CancellationToken>());
        Assert.False(canStartCall);
    }
}