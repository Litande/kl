using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace KL.Engine.Rule.Tests;

public class RuleEngineConditionsTests : RuleEngineConditionsTestsFixture
{
    [Fact]
    public async Task CurrentStatusCondition_ShouldBe_OneOfStatuses()
    {
        var leadStatuses = new[] { LeadStatusTypes.Busy, LeadStatusTypes.DNC };

        var leads = new[]
        {
            GetLead_Default(),
            GetLead_Default(),
            GetLead_Default(status: LeadStatusTypes.DNC),
            GetLead_Default(status: LeadStatusTypes.Busy),
        };
        var rules = CurrentStatus_Condition(ComparisonOperation.Is, leadStatuses).Combine();

        var results = await GetRuleProcessingService().ProcessConditions(leads, rules, RuleGroupTypes.ForwardRules);
        var resultConditions = results!.Values.Select(x => x.First()).ToList();

        // Assert
        Assert.False(resultConditions[0].IsSuccess);
        Assert.False(resultConditions[1].IsSuccess);
        Assert.True(resultConditions[2].IsSuccess);
        Assert.True(resultConditions[3].IsSuccess);
    }

    [Fact]
    public async Task CurrentStatusCondition_ShouldNotBe_OneOfStatuses()
    {
        var leadStatuses = new[] { LeadStatusTypes.NotInterested, LeadStatusTypes.DNC };

        var leads = new[]
        {
            GetLead_Default(status: LeadStatusTypes.NotInterested),
            GetLead_Default(status: LeadStatusTypes.DNC),
            GetLead_Default(status: LeadStatusTypes.Under18),
            GetLead_Default(status: LeadStatusTypes.CannotTalk),
        };

        var rules = CurrentStatus_Condition(ComparisonOperation.IsNot, leadStatuses).Combine();

        var results = await GetRuleProcessingService().ProcessConditions(leads, rules, RuleGroupTypes.ForwardRules);
        var resultConditions = results!.Values.Select(x => x.First()).ToList();

        // Assert
        Assert.False(resultConditions[0].IsSuccess);
        Assert.False(resultConditions[1].IsSuccess);
        Assert.True(resultConditions[2].IsSuccess);
        Assert.True(resultConditions[3].IsSuccess);
    }

    [Theory]
    [InlineData(1, 1, ComparisonOperation.EqualForLastYDays)]
    [InlineData(2, 2, ComparisonOperation.EqualForLastYDays)]
    [InlineData(3, 3, ComparisonOperation.EqualForLastYDays)]
    [InlineData(3, 4, ComparisonOperation.EqualForLastYDays)]
    [InlineData(0, 1, ComparisonOperation.NotEqualForLastYDays)]
    [InlineData(1, 2, ComparisonOperation.NotEqualForLastYDays)]
    [InlineData(4, 4, ComparisonOperation.NotEqualForLastYDays)]
    [InlineData(1, 2, ComparisonOperation.MoreThanForLastYDays)]
    [InlineData(2, 3, ComparisonOperation.MoreThanForLastYDays)]
    [InlineData(2, 1, ComparisonOperation.LessThanForLastYDays)]
    [InlineData(3, 2, ComparisonOperation.LessThanForLastYDays)]
    [InlineData(4, 4, ComparisonOperation.LessThanForLastYDays)]
    public async Task LeadHadTotalOfXCallsYPeriodCondition_ShouldHas_CallsForPeriod(
        int calls,
        int period,
        ComparisonOperation operation)
    {
        PrepareCdrRepository(useInMemoryDb: true);

        var leads = new[]
        {
            GetLead_Default(),
            GetLead_Default(),
            GetLead_Default(),
        };

        Fill_CallDetailRecords(leads, count: 3);

        var rules = LeadHadTotalOfXCallsYPeriod_Condition(operation, calls, period).Combine();

        var results = await GetRuleProcessingService().ProcessConditions(leads, rules, RuleGroupTypes.Behavior);
        var resultConditions = results!.Values.Select(x => x.First()).ToList();

        // Assert
        Assert.All(resultConditions, x => Assert.True(x.IsSuccess));
    }

    [Theory]
    [InlineData(1, 1, ComparisonOperation.EqualForLastYHours, 1, true)]
    [InlineData(2, 2, ComparisonOperation.NotEqualForLastYHours, 1, true)]
    [InlineData(2, 3, ComparisonOperation.MoreThanForLastYHours, 3, true)]
    [InlineData(3, 4, ComparisonOperation.LessThanForLastYHours, 2, true)]
    [InlineData(0, 1, ComparisonOperation.EqualForLastYDays, 1, false)]
    [InlineData(1, 2, ComparisonOperation.NotEqualForLastYDays, 0, true)]
    [InlineData(2, 4, ComparisonOperation.MoreThanForLastYDays, 4, true)]
    [InlineData(1, 2, ComparisonOperation.LessThanForLastYDays, 5, false)]
    public async Task NewStatusWasXConsecutiveTimesYPeriod_ShouldBe_Success(
        int countStatusChanges,
        int period,
        ComparisonOperation operation,
        int countStatusHistory,
        bool expectedValue)
    {
        var history = PrepareLeadStatusHistory(LeadStatusTypes.Busy, countStatusHistory);
        var leads = new[]
        {
            GetLead_Default(statusHistory: history, status: LeadStatusTypes.Busy),
        };

        var rules = NewStatusWasXConsecutiveTimesYPeriod_Condition(operation, countStatusChanges, period)
            .Combine();

        var results = await GetRuleProcessingService().ProcessConditions(leads, rules, RuleGroupTypes.Behavior);
        var resultConditions = results!.Values.Select(x => x.First()).ToList();

        Assert.Equal(expectedValue, resultConditions[0].IsSuccess);
    }

    [Theory]
    [InlineData(1, 1, ComparisonOperation.EqualForLastYHours, false)]
    [InlineData(2, 2, ComparisonOperation.NotEqualForLastYHours, true)]
    [InlineData(3, 3, ComparisonOperation.MoreThanForLastYHours, true)]
    [InlineData(3, 4, ComparisonOperation.LessThanForLastYHours, false)]
    [InlineData(0, 1, ComparisonOperation.EqualForLastYDays, false)]
    [InlineData(1, 2, ComparisonOperation.NotEqualForLastYDays, true)]
    [InlineData(4, 4, ComparisonOperation.MoreThanForLastYDays, true)]
    [InlineData(1, 2, ComparisonOperation.LessThanForLastYDays, false)]
    public async Task NewStatusWasTotalXTimesYPeriod_ShouldBe_Success(
        int countStatusChanges,
        int period,
        ComparisonOperation operation,
        bool expected)
    {
        const int countChangesStatus = 100;
        var history = PrepareLeadStatusHistory(LeadStatusTypes.Busy, countChangesStatus);
        var leads = new[]
        {
            GetLead_Default(statusHistory: history, status: LeadStatusTypes.Busy),
        };

        var rules = NewStatusWasTotalXTimesYPeriod_Condition(operation, countStatusChanges, period).Combine();

        var results = await GetRuleProcessingService().ProcessConditions(leads, rules, RuleGroupTypes.Behavior);
        var resultConditions = results!.Values.Select(x => x.First()).ToList();

        // Assert
        Assert.Equal(expected, resultConditions.First().IsSuccess);
    }

    [Fact]
    public async Task IsFutureCallCondition_ShouldBe_Success()
    {
        var leads = new[]
        {
            GetLead_Default(),
            GetLead_Default(),
            GetLead_Default(remindOn: DateTimeOffset.UtcNow),
            GetLead_Default(remindOn: DateTimeOffset.UtcNow),
        };
        var rules = IsFutureCall_Condition().Combine();

        var results = await GetRuleProcessingService().ProcessConditions(leads, rules, RuleGroupTypes.ForwardRules);
        var resultConditions = results!.Values.Select(x => x.First()).ToList();

        // Assert
        Assert.False(resultConditions[0].IsSuccess);
        Assert.False(resultConditions[1].IsSuccess);
        Assert.True(resultConditions[2].IsSuccess);
        Assert.True(resultConditions[3].IsSuccess);
    }

    [Theory]
    [InlineData(LeadStatusTypes.MaxCall)]
    [InlineData(LeadStatusTypes.Busy)]
    public async Task IsFixedAssignedFeedback_ShouldBe_Success(LeadStatusTypes status)
    {
        var leads = new[]
        {
            GetLead_Default(),
            GetLead_Default(),
            GetLead_Default(remindOn: DateTimeOffset.UtcNow, lastCallAgentId: 30, status: status),
            GetLead_Default(remindOn: DateTimeOffset.UtcNow, lastCallAgentId: 30, status: status, assignedAgentId: 30),
        };
        var rules = IsFixedAssignedFeedback_Condition(status).Combine();

        var results = await GetRuleProcessingService().ProcessConditions(leads, rules, RuleGroupTypes.Behavior);
        var resultConditions = results!.Values.Select(x => x.First()).ToList();

        // Assert
        Assert.False(resultConditions[0].IsSuccess);
        Assert.False(resultConditions[1].IsSuccess);
        Assert.True(resultConditions[2].IsSuccess);
        Assert.False(resultConditions[3].IsSuccess);
    }

    [Theory]
    [InlineData(ComparisonOperation.Is)]
    [InlineData(ComparisonOperation.IsNot)]
    public async Task IsTestLead_ShouldBe_Success(ComparisonOperation operation)
    {
        var leads = new[]
        {
            GetLead_Default(),
            GetLead_Default(),
            GetLead_Default(isTest: true),
            GetLead_Default(isTest: true),
        };
        var rules = IsTestLead_Condition(operation).Combine();

        var results = await GetRuleProcessingService().ProcessConditions(leads, rules, RuleGroupTypes.Behavior);
        var resultConditions = results!.Values.Select(x => x.First()).ToList();

        var isIsIsOperation = operation == ComparisonOperation.Is;

        // Assert
        Assert.Equal(isIsIsOperation, !resultConditions[0].IsSuccess);
        Assert.Equal(isIsIsOperation, !resultConditions[1].IsSuccess);
        Assert.Equal(isIsIsOperation, resultConditions[2].IsSuccess);
        Assert.Equal(isIsIsOperation, resultConditions[3].IsSuccess);
    }

    [Fact]
    public async Task IsImported_ShouldBe_Success()
    {
        var leads = new[]
        {
            GetLead_Default(),
            GetLead_Default(),
            GetLead_Default(systemStatus: LeadSystemStatusTypes.Imported),
            GetLead_Default(systemStatus: LeadSystemStatusTypes.Imported),
        };
        var rules = IsImported_Condition().Combine();

        var results = await GetRuleProcessingService().ProcessConditions(leads, rules, RuleGroupTypes.ForwardRules);
        var resultConditions = results!.Values.Select(x => x.First()).ToList();

        // Assert
        Assert.False(resultConditions[0].IsSuccess);
        Assert.False(resultConditions[1].IsSuccess);
        Assert.True(resultConditions[2].IsSuccess);
        Assert.True(resultConditions[3].IsSuccess);
    }

    [Theory]
    [InlineData(TimeUnits.Minutes, 30, ComparisonOperation.LessThan)]
    [InlineData(TimeUnits.Minutes, 60, ComparisonOperation.MoreThan)]
    [InlineData(TimeUnits.Hours, 2, ComparisonOperation.MoreThan)]
    [InlineData(TimeUnits.Days, 4, ComparisonOperation.Equal)]
    public async Task LeadIsInSystem_ShouldBe_Success(TimeUnits unit, int count, ComparisonOperation operation)
    {
        var r = new Random();

        DateTimeOffset GetRegistrationTime() => unit switch
        {
            TimeUnits.Minutes => DateTimeOffset.UtcNow.AddMinutes(-r.Next(1, 40)).AddSeconds(-10),
            TimeUnits.Hours => DateTimeOffset.UtcNow.AddHours(-r.Next(1, 10)).AddSeconds(-10),
            TimeUnits.Days => DateTimeOffset.UtcNow.AddDays(-r.Next(1, 5)).AddSeconds(-10),
            _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, null)
        };

        bool GetExceptedValue(TrackedLead lead)
        {
            bool Compare(DateTimeOffset registrationTime, TimeSpan span) => operation switch
            {
                ComparisonOperation.LessThan => registrationTime >= DateTimeOffset.UtcNow - span,
                ComparisonOperation.MoreThan => registrationTime <= DateTimeOffset.UtcNow - span,
                ComparisonOperation.Equal => registrationTime == DateTimeOffset.UtcNow - span,
                _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, null)
            };


            return unit switch
            {
                TimeUnits.Minutes => Compare(lead.RegistrationTime, TimeSpan.FromMinutes(count)),
                TimeUnits.Hours => Compare(lead.RegistrationTime, TimeSpan.FromHours(count)),
                TimeUnits.Days => Compare(lead.RegistrationTime, TimeSpan.FromDays(count)),
                _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, null)
            };
        }

        var leads = new[]
        {
            GetLead_Default(registrationTime: GetRegistrationTime()),
            GetLead_Default(registrationTime: GetRegistrationTime()),
            GetLead_Default(registrationTime: GetRegistrationTime()),
            GetLead_Default(registrationTime: GetRegistrationTime()),
        };

        var expectedValues = leads.Select(GetExceptedValue).ToList();

        var rules = LeadIsInTheSystem_Condition(operation, count, unit).Combine();

        var results = await GetRuleProcessingService().ProcessConditions(leads, rules, RuleGroupTypes.ForwardRules);
        var resultConditions = results!.Values.Select(x => x.First().IsSuccess).ToList();

        // Assert
        for (int i = 0; i < results.Count; i++)
            Assert.Equal(expectedValues[i], resultConditions[i]);
    }


    [Theory]
    [InlineData(ComparisonOperation.Is, "UA", "PL")]
    [InlineData(ComparisonOperation.IsNot, "US", "GB")]
    public async Task Country_ShouldBe_Success(ComparisonOperation operation, params string[] countryCodes)
    {
        var leads = new[]
        {
            GetLead_Default(countryCode: "UA"),
            GetLead_Default(countryCode: "PL"),
            GetLead_Default(countryCode: "UA"),
            GetLead_Default(countryCode: "UA"),
        };


        var rules = Country_Condition(operation, countryCodes).Combine();

        var results = await GetRuleProcessingService().ProcessConditions(leads, rules, RuleGroupTypes.ForwardRules);
        var resultConditions = results!.Values.Select(x => x.First().IsSuccess).ToList();

        // Assert
        Assert.All(resultConditions, Assert.True);
    }

    [Theory]
    [InlineData(ComparisonOperation.Equal, 200)]
    [InlineData(ComparisonOperation.LessThan, 300)]
    [InlineData(ComparisonOperation.MoreThan, 150)]
    public async Task CallDuration_ShouldBe_Success(ComparisonOperation operation, int seconds)
    {
        PrepareCdrRepository(useInMemoryDb: true);
        Clear_CDR();

        var leads = new[]
        {
            GetLead_Default(countryCode: "UA"),
            GetLead_Default(countryCode: "PL"),
        };

        foreach (var lead in leads)
        {
            Create_CallDetailRecords(
                new CallDetailRecord
                {
                    LeadId = lead.LeadId,
                    Id = long.Parse($"{lead.LeadId}1"),
                    CallDuration = 100,
                    UserAnswerAt = DateTimeOffset.UtcNow.AddDays(-7),
                    CallHangupAt = DateTimeOffset.UtcNow.AddDays(-6),
                },
                new CallDetailRecord
                {
                    LeadId = lead.LeadId,
                    Id = long.Parse($"{lead.LeadId}2"),
                    CallDuration = 100,
                    UserAnswerAt = DateTimeOffset.UtcNow.AddDays(-7),
                    CallHangupAt = DateTimeOffset.UtcNow.AddDays(-6),
                }
            );
        }


        var rules = CallDuration_Condition(operation, seconds).Combine();

        var results = await GetRuleProcessingService().ProcessConditions(leads, rules, RuleGroupTypes.Behavior);
        var resultConditions = results!.Values.Select(x => x.First().IsSuccess).ToList();

        // Assert
        Assert.All(resultConditions, Assert.True);
    }

    [Fact]
    public async Task NewCampaignLead_ShouldBe_Success()
    {
        var leads = new[]
        {
            GetLead_Default(status: LeadStatusTypes.NA),
            GetLead_Default(status: LeadStatusTypes.Busy),
            GetLead_Default(status: LeadStatusTypes.NewLead),
            GetLead_Default(status: LeadStatusTypes.NewLead),
        };
        var rules = NewCampaignLead_Condition().Combine();

        var results = await GetRuleProcessingService().ProcessConditions(leads, rules, RuleGroupTypes.Behavior);
        var resultConditions = results!.Values.Select(x => x.First()).ToList();

        // Assert
        Assert.False(resultConditions[0].IsSuccess);
        Assert.False(resultConditions[1].IsSuccess);
        Assert.True(resultConditions[2].IsSuccess);
        Assert.True(resultConditions[3].IsSuccess);
    }

    [Theory]
    [InlineData(ComparisonOperation.Equal, 200)]
    [InlineData(ComparisonOperation.LessThan, 300)]
    [InlineData(ComparisonOperation.MoreThan, 150)]
    [InlineData(ComparisonOperation.NotEqual, 150)]
    public async Task LeadHadTotalOfXCalls_ShouldBe_Success(ComparisonOperation operation, int seconds)
    {
        PrepareCdrRepository(useInMemoryDb: true);
        Clear_CDR();

        var leads = new[]
        {
            GetLead_Default(status: LeadStatusTypes.NA),
        };

        const int callDuration = 100;

        Create_CallDetailRecords(
            new CallDetailRecord
            {
                LeadId = leads[0].LeadId,
                Id = long.Parse($"{leads[0].LeadId}1"),
                CallDuration = callDuration,
                UserAnswerAt = DateTimeOffset.UtcNow.AddDays(-7),
                CallHangupAt = DateTimeOffset.UtcNow.AddDays(-6),
            });

        var rules = LeadHadTotalOfXCalls_Condition(operation, seconds).Combine();

        var results = await GetRuleProcessingService().ProcessConditions(leads, rules, RuleGroupTypes.Behavior);
        var resultConditions = results!.Values.Select(x => x.First()).ToList();

        var expectedValue = operation switch
        {
            ComparisonOperation.Equal => callDuration == seconds,
            ComparisonOperation.NotEqual => callDuration != seconds,
            ComparisonOperation.MoreThan => callDuration > seconds,
            ComparisonOperation.LessThan => callDuration < seconds,
            _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
        };

        // Assert
        Assert.Equal(expectedValue, resultConditions.First().IsSuccess);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task CampaignLeadAssignedToAgent_ShouldBe_Success(bool isEquals)
    {
        var leads = new[]
        {
            GetLead_Default(assignedAgentId: 30),
        };

        var rules = CampaignLeadAssignedToAgent_Condition(isEquals).Combine();

        var results = await GetRuleProcessingService().ProcessConditions(leads, rules, RuleGroupTypes.Behavior);
        var resultConditions = results!.Values.Select(x => x.First()).ToList();

        var lead = leads.First();

        var expectedValue = lead.AssignedAgentId != null == isEquals;

        // Assert
        Assert.Equal(expectedValue, resultConditions.First().IsSuccess);
    }

    [Theory]
    [InlineData(ComparisonOperation.Equal, 200)]
    [InlineData(ComparisonOperation.LessThan, 300)]
    [InlineData(ComparisonOperation.MoreThan, 150)]
    [InlineData(ComparisonOperation.NotEqual, 150)]
    public async Task NewStatusWasXConsecutiveTimes_ShouldBe_Success(ComparisonOperation operation, int count)
    {
        const int countChangesStatus = 100;
        var history = PrepareLeadStatusHistory(LeadStatusTypes.Busy, countChangesStatus);
        var leads = new[]
        {
            GetLead_Default(statusHistory: history, status: LeadStatusTypes.Busy),
        };

        var rules = NewStatusWasXConsecutiveTimes_Condition(operation, count).Combine();

        var results = await GetRuleProcessingService().ProcessConditions(leads, rules, RuleGroupTypes.Behavior);
        var resultConditions = results!.Values.Select(x => x.First()).ToList();

        var expectedValue = operation switch
        {
            ComparisonOperation.Equal => countChangesStatus == count,
            ComparisonOperation.NotEqual => countChangesStatus != count,
            ComparisonOperation.MoreThan => countChangesStatus > count,
            ComparisonOperation.LessThan => countChangesStatus < count,
            _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
        };

        // Assert
        Assert.Equal(expectedValue, resultConditions.First().IsSuccess);
    }

    [Theory]
    [InlineData(ComparisonOperation.Equal, 200)]
    [InlineData(ComparisonOperation.LessThan, 300)]
    [InlineData(ComparisonOperation.MoreThan, 150)]
    [InlineData(ComparisonOperation.NotEqual, 150)]
    public async Task NewStatusWasTotalXTimes_ShouldBe_Success(ComparisonOperation operation, int count)
    {
        const int countChangesStatus = 100;
        const LeadStatusTypes status = LeadStatusTypes.Busy;
        var history = PrepareLeadStatusHistory(status, countChangesStatus);
        var leads = new[]
        {
            GetLead_Default(statusHistory: history, status: status),
        };

        var rules = NewStatusWasTotalXTimes_Condition(operation, count).Combine();

        var results = await GetRuleProcessingService().ProcessConditions(leads, rules, RuleGroupTypes.Behavior);
        var resultConditions = results!.Values.Select(x => x.First()).ToList();

        var expectedValue = operation switch
        {
            ComparisonOperation.Equal => countChangesStatus == count,
            ComparisonOperation.NotEqual => countChangesStatus != count,
            ComparisonOperation.MoreThan => countChangesStatus > count,
            ComparisonOperation.LessThan => countChangesStatus < count,
            _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
        };

        // Assert
        Assert.Equal(expectedValue, resultConditions.First().IsSuccess);
    }


    [Theory]
    [InlineData(ComparisonOperation.Equal, 200)]
    [InlineData(ComparisonOperation.LessThan, 300)]
    [InlineData(ComparisonOperation.MoreThan, 150)]
    [InlineData(ComparisonOperation.NotEqual, 150)]
    public async Task CampaignLeadWeightIs_ShouldBe_Success(ComparisonOperation operation, int count)
    {
        const long score = 2000;
        var leads = new[]
        {
            GetLead_Default(score: score),
        };

        var rules = CampaignLeadWeightIs_Condition(operation, count).Combine();

        var results = await GetRuleProcessingService().ProcessConditions(leads, rules, RuleGroupTypes.Behavior);
        var resultConditions = results!.Values.Select(x => x.First()).ToList();

        var expectedValue = operation switch
        {
            ComparisonOperation.Equal => score == count,
            ComparisonOperation.NotEqual => score != count,
            ComparisonOperation.MoreThan => score > count,
            ComparisonOperation.LessThan => score < count,
            _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
        };

        // Assert
        Assert.Equal(expectedValue, resultConditions.First().IsSuccess);
    }

    [Theory]
    [InlineData(ComparisonOperation.Is, LeadStatusTypes.Busy)]
    [InlineData(ComparisonOperation.IsNot, LeadStatusTypes.Busy)]
    public async Task NewStatus_ShouldBe_Success(ComparisonOperation operation, LeadStatusTypes status)
    {
        const LeadStatusTypes leadStatus = LeadStatusTypes.Busy;
        var leads = new[]
        {
            GetLead_Default(status: leadStatus),
        };

        var rules = NewStatus_Condition(operation, status).Combine();

        var results = await GetRuleProcessingService().ProcessConditions(leads, rules, RuleGroupTypes.Behavior);
        var resultConditions = results!.Values.Select(x => x.First()).ToList();

        var expectedValue = operation switch
        {
            ComparisonOperation.Is => leadStatus == status,
            ComparisonOperation.IsNot => leadStatus != status,
            _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
        };

        // Assert
        Assert.Equal(expectedValue, resultConditions.First().IsSuccess);
    }

    [Theory]
    [InlineData(ComparisonOperation.Is, LeadStatusTypes.Busy)]
    [InlineData(ComparisonOperation.IsNot, LeadStatusTypes.Busy)]
    public async Task PreviousStatus_ShouldBe_Success(ComparisonOperation operation, LeadStatusTypes status)
    {
        var history = PrepareLeadStatusHistory(LeadStatusTypes.Under18, 5);

        var leads = new[]
        {
            GetLead_Default(statusHistory: history),
        };

        var rules = PreviousStatus_Condition(operation, status).Combine();

        var results = await GetRuleProcessingService().ProcessConditions(leads, rules, RuleGroupTypes.Behavior);
        var resultConditions = results!.Values.Select(x => x.First()).ToList();

        var expectedValue = operation switch
        {
            ComparisonOperation.Is => ConditionsHelper.IsPreviousStatus(leads.First(), status),
            ComparisonOperation.IsNot => ConditionsHelper.IsPreviousStatus(leads.First(), status) == false,
            _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
        };

        // Assert
        Assert.Equal(expectedValue, resultConditions.First().IsSuccess);
    }
}