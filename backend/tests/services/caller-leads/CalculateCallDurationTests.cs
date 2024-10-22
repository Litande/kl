using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace KL.Caller.Leads.Tests;

public class CalculateCallDurationTests
{
    private readonly DbContextOptions<DialDbContext> _dbContextOptions;

    public CalculateCallDurationTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<DialDbContext>()
            .UseInMemoryDatabase(databaseName: "DialDbContextInMemory")
            .Options;
    }

    [Theory]
    [InlineData(150, 100, 50, 3000)]
    [InlineData(150, null, 0, null)]
    [InlineData(null, null, 100, null)]
    [InlineData(null, 100, 100, null)]
    [InlineData(null, null, 0, null)]
    [InlineData(0, null, 1000, null)]
    [InlineData(90, 100, 50, 2400)]
    public async Task LeadQueue_FutureCalls_MaxPriority_ShouldBeFirst(
        int? leadAnsweredMinutesAgo,
        int? agentAnsweredMinutesAgo,
        int callHangupMinutesAgo,
        long? expectedCallDurationSeconds)
    {
        var userRepositoryMock = new Mock<IUserRepository>();
        await using var context = new DialDbContext(_dbContextOptions);
        var repo = new CDRRepository(context, userRepositoryMock.Object);
        var rand = new Random();

        var message = new CallFinishedMessage(
            ClientId: 1,
            BridgeId: 10 + 1.ToString(),
            SessionId: 100 + 1.ToString(),
            QueueId: 1,
            CallType: CallType.Manual,
            AgentId: 1,
            LeadId: null,
            LeadPhone: 1000 + 1.ToString(),
            ReasonCode: (CallFinishReasons)rand.Next(201, 208),
            ReasonDetails: null,
            SipProviderId: 1,
            SipErrorCode: null,
            CallOriginatedAt: DateTimeOffset.UtcNow.AddMinutes(-200),
            LeadAnswerAt: leadAnsweredMinutesAgo is null ? null : DateTimeOffset.UtcNow.AddMinutes(-leadAnsweredMinutesAgo.Value),
            AgentAnswerAt: agentAnsweredMinutesAgo is null ? null : DateTimeOffset.UtcNow.AddMinutes(-agentAnsweredMinutesAgo.Value),
            CallHangupAt: DateTimeOffset.UtcNow.AddMinutes(-callHangupMinutesAgo)
            );

        var callDurationInSeconds = repo.CalculateCallDuration(message);

        Assert.Equal(expectedCallDurationSeconds, callDurationInSeconds);
    }
}
