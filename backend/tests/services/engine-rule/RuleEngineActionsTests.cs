using System;
using Plat4Me.DialRuleEngine.Application.Enums;
using System.Threading.Tasks;
using Plat4Me.DialRuleEngine.Application.Models;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Enums;
using Xunit;

namespace Plat4Me.DialRuleEngine.Tests;

public class RuleEngineActionsTests : RuleEngineActionsTestsFixture
{
    [Fact]
    public async Task AssignAgentAction_ShouldSet_LastCallAgentId_To_AssignedAgentId()
    {
        var leads = new[]
        {
            GetLead_Default(lastCallAgentId: 1),
            GetLead_Default(lastCallAgentId: 2),
        };
        var rules = AssignAgent_Action().Combine();

        await GetRuleProcessingService().Process(1, leads, rules, RuleGroupTypes.Behavior);

        // Assert
        Assert.Equal(1, leads[0].LastCallAgentId);
        Assert.Equal(2, leads[1].LastCallAgentId);
        Assert.All(leads, r => Assert.Equal(r.LastCallAgentId, r.AssignedAgentId));
    }

    [Theory]
    [InlineData(11)]
    [InlineData(101)]
    public async Task AssignAgentAction_ShouldSet_LeadQueueId(long queueId)
    {
        var leads = new[]
        {
            GetLead_Default(),
            GetLead_Default(queueId: queueId),
        };
        var rules = MoveLeadToQueue_Action(queueId).Combine();

        await GetRuleProcessingService().Process(1, leads, rules, RuleGroupTypes.ForwardRules);

        // Assert
        Assert.All(leads, r => Assert.Equal(queueId, r.LeadQueueId));
    }

    [Theory]
    [InlineData(5, ActionOperation.Increase, 5)]
    [InlineData(10, ActionOperation.Decrease, -10)]
    [InlineData(15, ActionOperation.Set, 15)]
    public async Task SetLeadWeightAction_ShouldSet_Weight(long score, ActionOperation operation, long expected)
    {
        var leads = new[]
        {
            GetLead_Default(score: 0),
        };

        var rules = SetLeadWeightByX_Action(operation, score).Combine();

        await GetRuleProcessingService().Process(1, leads, rules, RuleGroupTypes.LeadScoring);

        // Assert
        Assert.Equal(expected, leads[0].Score);
    }

    [Theory]
    [InlineData(nameof(TrackedLead.Score), 100)]
    // [InlineData(nameof(TrackedLead.LeadQueueId), 5)]
    [InlineData(nameof(TrackedLead.LeadPhone), "380964567890")]
    [InlineData(nameof(TrackedLead.FirstName), "Tony")]
    [InlineData(nameof(TrackedLead.LastName), "Adam")]
    // [InlineData(nameof(TrackedLead.RegistrationTime), "27.04.2023 13:39:17 +00:00")]
    public async Task ChangeLeadFieldsValueAction_ShouldSet_ValueByFieldName(string fieldName, object value)
    {
        var strValue = value.ToString()!;
        var leads = new[]
        {
            GetLead_Default(score: 10, queueId: 1, timezone: "Europe/Kiev"),
        };

        var rules = ChangeLeadFieldsValue_Action(fieldName, strValue).Combine();

        await GetRuleProcessingService().Process(1, leads, rules, RuleGroupTypes.Behavior);

        var lead = leads[0];

        switch (fieldName)
        {
            case nameof(TrackedLead.Score):
                Assert.Equal(long.Parse(strValue), lead.Score);
                break;
            case nameof(TrackedLead.LeadQueueId):
                Assert.Equal(long.Parse(strValue), lead.LeadQueueId);
                break;
            case nameof(TrackedLead.LeadPhone):
                Assert.Equal(strValue, lead.LeadPhone);
                break;
            case nameof(TrackedLead.FirstName):
                Assert.Equal(strValue, lead.FirstName);
                break;
            case nameof(TrackedLead.LastName):
                Assert.Equal(strValue, lead.LastName);
                break;
            case nameof(TrackedLead.RegistrationTime):
                Assert.Equal(strValue, lead.RegistrationTime.ToString());
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(fieldName), fieldName, null);
        }
    }

    [Fact]
    public async Task DeleteLeadAction_ShouldSet_DeletedOn()
    {
        var leads = new[]
        {
            GetLead_Default(),
            GetLead_Default(),
            GetLead_Default(),
            GetLead_Default(),
        };

        var rules = DeleteLead_Action().Combine();

        await GetRuleProcessingService().Process(1, leads, rules, RuleGroupTypes.Behavior);

        // Assert
        Assert.All(leads, x => Assert.NotNull(x.DeletedOn));
    }

    [Theory]
    [InlineData(LeadStatusTypes.NewLead)]
    [InlineData(LeadStatusTypes.DNC)]
    [InlineData(LeadStatusTypes.DNCCountry)]
    public async Task ChangeActionAction_ShouldSet_Status(LeadStatusTypes status)
    {
        var leads = new[]
        {
            GetLead_Default(status: LeadStatusTypes.Busy),
            GetLead_Default(status: LeadStatusTypes.Busy),
            GetLead_Default(status: LeadStatusTypes.Busy),
            GetLead_Default(status: LeadStatusTypes.Busy),
        };
        var rules = ChangeStatus_Action(status).Combine();

        await GetRuleProcessingService().Process(1, leads, rules, RuleGroupTypes.Behavior);

        // Assert
        Assert.All(leads, x => Assert.Equal(status, x.Status));
    }

    [Theory]
    [InlineData(10, TimeUnits.Minutes)]
    [InlineData(5, TimeUnits.Hours)]
    [InlineData(1, TimeUnits.Days)]
    public async Task FreezeLeadForX_ShouldSet_FreezeTo(int count, TimeUnits units)
    {
        var leads = new[]
        {
            GetLead_Default(),
            GetLead_Default(),
            GetLead_Default(),
            GetLead_Default(),
        };

        var rules = FreezeLeadForX_Action(count, units).Combine();

        await GetRuleProcessingService().Process(1, leads, rules, RuleGroupTypes.Behavior);

        var rangeTime = (units switch
        {
            TimeUnits.Minutes => TimeSpan.FromMinutes(count),
            TimeUnits.Hours => TimeSpan.FromHours(count),
            TimeUnits.Days => TimeSpan.FromDays(count),
            _ => throw new ArgumentOutOfRangeException(nameof(units), units, null)
        }).Add(TimeSpan.FromSeconds(15));

        bool IsTimeInRange(DateTimeOffset? time)
        {
            if (!time.HasValue) return false;

            var diff = time - DateTimeOffset.UtcNow;
            var isTimeInRange = rangeTime >= diff && diff <= rangeTime;
            return isTimeInRange;
        }

        // Assert
        Assert.All(leads, x => Assert.True(IsTimeInRange(x.FreezeTo)));
    }
}