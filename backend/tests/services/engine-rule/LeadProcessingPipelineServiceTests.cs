using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using KL.Engine.Rule.Enums;
using KL.Engine.Rule.Models;
using KL.Engine.Rule.Repositories;
using KL.Engine.Rule.RuleEngine.Enums;
using KL.Engine.Rule.RuleEngine.MicrosoftEngine;
using Xunit;

namespace KL.Engine.Rule.Tests;

public class LeadProcessingPipelineServiceTests : LeadProcessingPipelineServiceTestsFixture
{
    [Theory]
    [InlineData("true")]
    [InlineData("false")]
    public async Task CampaignLeadAssignedToAgentTest_LeadWithAssignedAgent_ShouldChangeStatus(string selectValue)
    {
        var ruleService = new MicrosoftRuleEngineProcessingService(
            RuleServiceLoggerMock.Object,
            EngineMapper.Object,
            CDRRepositoryMock.Object,
            EmptySettingsRepositoryMock.Object
        );
        var leads = new List<TrackedLead>();
        var rand = new Random();
        for (var i = 1; i <= 3; i++)
        {
            var item = new TrackedLead
            (
                i,
                "First_" + i,
                "Last_" + i,
                "Phone_" + i,
                (LeadStatusTypes)rand.Next(1, 29),
                null, DateTimeOffset.UtcNow, null,
                null,
                null, null, null, false,
                assignedAgentId: i == 1 ? 100 : null,
                null, null, null
            )
            {
                Status = LeadStatusTypes.Busy,
            };
            leads.Add(item);
        }

        var rules = GetQueueFakeRulesCampaignLeadAssignedToAgent(selectValue);

        await ruleService.Process(1,
            leads,
            rules,
            RuleGroupTypes.Behavior);

        var toggle = selectValue == "true";
        var result = leads.Where(r => r.LeadId == 1 == toggle);

        Assert.Contains(result, r => r.AssignedAgentId == 100 == toggle);
        Assert.Contains(result, r => r.Status == LeadStatusTypes.MaxCall);
    }

    [Fact]
    public async Task ConditionsHelperTest()
    {
        var rand = new Random();
        var statusChangeHistory1 = new List<ValueChanges<object?>>
            { new("Status", LeadStatusTypes.CallAgainGeneral, LeadStatusTypes.Busy) };
        var statusChangeHistory2 = new List<ValueChanges<object?>>
            { new("Status", LeadStatusTypes.DNC, LeadStatusTypes.MaxCall) };
        var statusChangeHistoryDto1 = new LeadHistoryChangesDto<object?> { Properties = statusChangeHistory1 };
        var statusChangeHistoryDto2 = new LeadHistoryChangesDto<object?> { Properties = statusChangeHistory2 };

        var jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        var statusHistoryArray = new List<KeyValuePair<DateTimeOffset, string>>
        {
            new(DateTimeOffset.UtcNow.AddDays(0),
                JsonSerializer.Serialize(statusChangeHistoryDto1, jsonSerializerOptions)),
            new(DateTimeOffset.UtcNow.AddDays(-1),
                JsonSerializer.Serialize(statusChangeHistoryDto1, jsonSerializerOptions)),
            new(DateTimeOffset.UtcNow.AddDays(-2),
                JsonSerializer.Serialize(statusChangeHistoryDto2, jsonSerializerOptions)),
            new(DateTimeOffset.UtcNow.AddDays(-3),
                JsonSerializer.Serialize(statusChangeHistoryDto1, jsonSerializerOptions)),
            new(DateTimeOffset.UtcNow.AddDays(-4),
                JsonSerializer.Serialize(statusChangeHistoryDto1, jsonSerializerOptions)),
            new(DateTimeOffset.UtcNow.AddDays(-5),
                JsonSerializer.Serialize(statusChangeHistoryDto1, jsonSerializerOptions)),
        };

        var lead = new TrackedLead
        (
            1,
            "First_" + 1,
            "Last_" + 1,
            "Phone_" + 1,
            LeadStatusTypes.Busy,
            null,
            new DateTimeOffset(), null, null, null, null, null, false, null, null,
            statusHistoryArray, null
        )
        {
            Score = rand.Next(100, 10000)
        };

        var result = ConditionsHelper.LastStatusConsecutiveTimes(lead);

        Assert.Equal(3, result);
    }

    [Fact]
    public async Task Process10kLeadsWithRuleEngine_AlwaysTrueSingleAct()
    {
        var ruleService = new MicrosoftRuleEngineProcessingService(
            RuleServiceLoggerMock.Object,
            EngineMapper.Object,
            CDRRepositoryMock.Object,
            EmptySettingsRepositoryMock.Object
        );
        var leads = new List<TrackedLead>();
        var rand = new Random();
        for (var i = 1; i <= 10000; i++)
        {
            leads.Add(new TrackedLead
                (
                    0,
                    "First_" + i,
                    "Last_" + i,
                    "Phone_" + i,
                    (LeadStatusTypes)rand.Next(1, 29),
                    null,
                    new DateTimeOffset(), null, null, null, null, null, false, null, null, null, null
                )
                {
                    Score = rand.Next(100, 10000)
                }
            );
        }


        var rules = GetQueueFakeRulesAlwaysTrue_ChangeStatusDNC();

        await ruleService.Process(1,
            leads,
            rules,
            RuleGroupTypes.Behavior
        );

        var resultBusyCnt = leads.Count(x => x.Status == LeadStatusTypes.DNC);
        Assert.Equal(leads.Count, resultBusyCnt);
    }

    [Fact]
    public async Task Process10kLeadsWithRuleEngine_OrCondSingleAct()
    {
        var ruleService = new MicrosoftRuleEngineProcessingService(
            RuleServiceLoggerMock.Object,
            EngineMapper.Object,
            CDRRepositoryMock.Object,
            EmptySettingsRepositoryMock.Object
        );
        var leads = new List<TrackedLead>();
        var rand = new Random();
        for (var i = 1; i <= 10000; i++)
        {
            var status = (LeadStatusTypes)rand.Next(1, 29);
            leads.Add(new TrackedLead
                (
                    0,
                    "First_" + i,
                    "Last_" + i,
                    "Phone_" + i,
                    status,
                    null,
                    DateTimeOffset.UtcNow, null, null, null, null, null, false, null, null, 
                    PrepareLeadStatusHistory(status, 2)
                    , null
                )
                {
                    Score = rand.Next(100, 10000)
                }
            );
        }

        var initCondCnt = leads.Count(x => x.Status == LeadStatusTypes.Busy || x.Status == LeadStatusTypes.CheckNumber);
        var initDNCCnt = leads.Count(x => x.Status == LeadStatusTypes.DNC);

        var rules = GetQueueFakeRulesPreviousStatusBusyOrCheckNumber_ChangeStatusDNC();

        await ruleService.Process(1,
            leads,
            rules,
            RuleGroupTypes.Behavior
        );

        var resultBusyCnt = leads.Count(x => x.Status == LeadStatusTypes.DNC);
        Assert.Equal(initCondCnt + initDNCCnt, resultBusyCnt);
    }

    [Fact]
    public async Task Process10kLeadsWithRuleEngine_AndCondSingleAct()
    {
        var ruleService = new MicrosoftRuleEngineProcessingService(
            RuleServiceLoggerMock.Object,
            EngineMapper.Object,
            CDRRepositoryMock.Object,
            EmptySettingsRepositoryMock.Object
        );
        var leads = new List<TrackedLead>();
        var rand = new Random();
        for (var i = 1; i <= 10000; i++)
        {
            var status = (LeadStatusTypes)rand.Next(1, 29);
            leads.Add(new TrackedLead
                (
                    0,
                    "First_" + i,
                    "Last_" + i,
                    "Phone_" + i,
                    status,
                    null,
                    DateTimeOffset.UtcNow, null, null, null, null, null, false, null, null,
                    PrepareLeadStatusHistory(status, 2),
                    null
                )
                {
                    Score = rand.Next(100, 10000)
                }
            );
        }

        var initBusy1000Cnt = leads.Count(x => x.Status == LeadStatusTypes.Busy && x.Score > 1000);
        var initDNCCnt = leads.Count(x => x.Status == LeadStatusTypes.DNC);

        var rules = GetQueueFakeRulesPreviousStatusBusyAndScoreMore1000_ChangeStatusDNC();

        await ruleService.Process(1,
            leads,
            rules,
            RuleGroupTypes.Behavior
        );

        var resultBusyCnt = leads.Count(x => x.Status == LeadStatusTypes.DNC);
        Assert.Equal(initBusy1000Cnt + initDNCCnt, resultBusyCnt);
    }

    [Fact]
    public async Task Process10kLeadsWithRuleEngine_AndCondSubOrSingleAct()
    {
        var ruleService = new MicrosoftRuleEngineProcessingService(
            RuleServiceLoggerMock.Object,
            EngineMapper.Object,
            CDRRepositoryMock.Object,
            EmptySettingsRepositoryMock.Object
        );
        var leads = new List<TrackedLead>();
        var rand = new Random();
        for (var i = 1; i <= 10000; i++)
        {
            var status = (LeadStatusTypes)rand.Next(1, 29);
            leads.Add(new TrackedLead
                (
                    0,
                    "First_" + i,
                    "Last_" + i,
                    "Phone_" + i,
                    status,
                    null,
                    DateTimeOffset.UtcNow, null, null, null, null, null, false, null, null,
                    PrepareLeadStatusHistory(status, 2),
                    null
                )
                {
                    Score = rand.Next(100, 10000)
                }
            );
        }

        var initBusy1000Cnt = leads.Count(x => x.Status == LeadStatusTypes.Busy && (x.Score < 1000 || x.Score > 5000));
        var initDNCCnt = leads.Count(x => x.Status == LeadStatusTypes.DNC);

        var rules = GetQueueFakeRulesPreviousStatusBusyAndScoreMore5000OrLess1000_ChangeStatusDNC();

        await ruleService.Process(1,
            leads,
            rules,
            RuleGroupTypes.Behavior
        );

        var resultCnt = leads.Count(x => x.Status == LeadStatusTypes.DNC);
        Assert.Equal(initBusy1000Cnt + initDNCCnt, resultCnt);
    }

    [Theory]
    [InlineData(3, 3, ComparisonOperation.EqualForLastYDays)]
    [InlineData(3, 4, ComparisonOperation.EqualForLastYDays)]
    [InlineData(2, 1, ComparisonOperation.EqualForLastYDays)]
    [InlineData(1, null, ComparisonOperation.MoreThan)]
    [InlineData(1, 25, ComparisonOperation.MoreThanForLastYHours)]
    public async Task Process10kLeadsWithRuleEngine_NewStatusWasXTimes(int x, int? y, ComparisonOperation operation)
    {
        var ruleService = new MicrosoftRuleEngineProcessingService(
            RuleServiceLoggerMock.Object,
            EngineMapper.Object,
            CDRRepositoryMock.Object,
            EmptySettingsRepositoryMock.Object
        );
        var leads = new List<TrackedLead>();
        var rand = new Random();
        for (var i = 1; i <= 3; i++)
        {
            var statusChangeHistory = new LeadHistoryChangesDto<object?>
            {
                Properties = new List<ValueChanges<object?>>
                    { new("Status", LeadStatusTypes.CallAgainGeneral, LeadStatusTypes.Busy) }
            };
            var statusHistoryArray = new List<KeyValuePair<DateTimeOffset, string>>();

            if (i == 1)
            {
                statusHistoryArray.Add(new KeyValuePair<DateTimeOffset, string>(DateTimeOffset.UtcNow,
                    JsonSerializer.Serialize(statusChangeHistory, SerializerOptions)));
                statusHistoryArray.Add(new KeyValuePair<DateTimeOffset, string>(DateTimeOffset.UtcNow.AddDays(-1),
                    JsonSerializer.Serialize(statusChangeHistory, SerializerOptions)));
                statusHistoryArray.Add(new KeyValuePair<DateTimeOffset, string>(DateTimeOffset.UtcNow.AddDays(-2),
                    JsonSerializer.Serialize(statusChangeHistory, SerializerOptions)));
            }

            var item = new TrackedLead
            (
                i,
                "First_" + i,
                "Last_" + i,
                "Phone_" + i,
                (LeadStatusTypes)rand.Next(1, 29),
                null,
                DateTimeOffset.UtcNow,
                null, null, null, null, null, false, null, null,
                statusHistoryArray, null
            )
            {
                Status = LeadStatusTypes.Busy,
            };
            leads.Add(item);
        }

        var rules = GetQueueFakeRulesNewStatusWasXTimes_ChangeToMaxCall(x, y, operation);

        await ruleService.Process(1,
            leads,
            rules,
            RuleGroupTypes.Behavior
        );

        var resultCnt = leads.First(r => r.LeadId == 1).Status;
        Assert.Equal(LeadStatusTypes.MaxCall, resultCnt);
    }

    [Fact]
    public async Task Process10kLeadsWithRuleEngine_NewLead()
    {
        var ruleService = new MicrosoftRuleEngineProcessingService(
            RuleServiceLoggerMock.Object,
            EngineMapper.Object,
            CDRRepositoryMock.Object,
            EmptySettingsRepositoryMock.Object
        );
        var leads = new List<TrackedLead>();
        var rand = new Random();
        for (var i = 1; i <= 10000; i++)
        {
            leads.Add(new TrackedLead
                (
                    0,
                    "First_" + i,
                    "Last_" + i,
                    "Phone_" + i,
                    (LeadStatusTypes)rand.Next(1, 29),
                    null,
                    DateTimeOffset.UtcNow, null, null, null, null, null, false, null, null, null, null
                )
                {
                    Score = rand.Next(100, 10000)
                }
            );
        }

        var initCnt = leads.Count(x => x.Status == LeadStatusTypes.NewLead);

        var rules = GetQueueFakeRulesNewLead_SetLeadWeightByXSet20k();

        await ruleService.Process(1,
            leads,
            rules,
            RuleGroupTypes.LeadScoring
        );

        var resultCnt = leads.Count(x => x.Score == 20000);
        Assert.Equal(initCnt, resultCnt);
    }

    [Fact]
    public async Task Process10kLeadsWithRuleEngine_LeadScoreLess2k()
    {
        var ruleService = new MicrosoftRuleEngineProcessingService(
            RuleServiceLoggerMock.Object,
            EngineMapper.Object,
            CDRRepositoryMock.Object,
            EmptySettingsRepositoryMock.Object
        );
        var leads = new List<TrackedLead>();
        var rand = new Random();
        for (var i = 1; i <= 10000; i++)
        {
            leads.Add(new TrackedLead
                (
                    0,
                    "First_" + i,
                    "Last_" + i,
                    "Phone_" + i,
                    (LeadStatusTypes)rand.Next(1, 29),
                    null,
                    DateTimeOffset.UtcNow, null, null, null, null, null, false, null, null, null, null
                )
                {
                    Score = rand.Next(100, 10000)
                }
            );
        }

        var targetLeads = leads.Where(x => x.Score < 2000).ToList();

        var rules = GetQueueFakeRulesLeadWeightIsLess2k_SetLeadWeightByXInc2k();

        await ruleService.Process(1,
            leads,
            rules,
            RuleGroupTypes.LeadScoring);

        Assert.DoesNotContain(targetLeads, x => x.Score < 2000);
    }

    [Fact]
    public async Task Process10kLeadsWithRuleEngine_CountryUS()
    {
        var ruleService = new MicrosoftRuleEngineProcessingService(
            RuleServiceLoggerMock.Object,
            EngineMapper.Object,
            CDRRepositoryMock.Object,
            EmptySettingsRepositoryMock.Object
        );
        var leads = new List<TrackedLead>();
        var rand = new Random();
        var countries = new[] { "US", "UA", "UK", "GB", "DE", "XX" };
        for (var i = 1; i <= 1000; i++)
        {
            leads.Add(new TrackedLead
                (
                    0,
                    "First_" + i,
                    "Last_" + i,
                    "Phone_" + i,
                    LeadStatusTypes.NewLead,
                    null,
                    DateTimeOffset.UtcNow, null, null, countries[rand.Next(countries.Length)], null, null, false, null,
                    null, null, null
                )
                {
                    Score = rand.Next(100, 10000)
                }
            );
        }

        var dncLeadsCount = leads.Count(x => x.CountryCode is "US" or "UK");

        var rules = GetQueueFakeRulesCountryUS_ChangeStatusDNC_Multiple();

        await ruleService.Process(1,
            leads,
            rules,
            RuleGroupTypes.Behavior);

        var resultCnt = leads.Count(x => x.Status == LeadStatusTypes.DNC);
        Assert.Equal(dncLeadsCount, resultCnt);
    }

    [Fact]
    public async Task Process10kLeadsWithRuleEngine_ScoreMore5000_Delete()
    {
        var ruleService = new MicrosoftRuleEngineProcessingService(
            RuleServiceLoggerMock.Object,
            EngineMapper.Object,
            CDRRepositoryMock.Object,
            EmptySettingsRepositoryMock.Object
        );
        var leads = new List<TrackedLead>();
        var rand = new Random();
        for (var i = 1; i <= 10000; i++)
        {
            leads.Add(new TrackedLead
                (
                    0,
                    "First_" + i,
                    "Last_" + i,
                    "Phone_" + i,
                    (LeadStatusTypes)rand.Next(1, 29),
                    null,
                    DateTimeOffset.UtcNow, null, null, null, null, null, false, null, null, null, null
                )
                {
                    Score = rand.Next(100, 10000)
                }
            );
        }

        var initCnt = leads.Count(x => x.Score > 5000);

        var rules = GetQueueFakeRulesLeadWeightMore5k_DeleteLead();

        await ruleService.Process(1,
            leads,
            rules,
            RuleGroupTypes.Behavior);

        var resultCnt = leads.Count(x => x.DeletedOn != null);
        Assert.Equal(initCnt, resultCnt);
    }

    [Fact]
    public async Task Process10kLeadsWithRuleEngine_StatusIsCallAgainPersonal_AssignAgent()
    {
        const LeadStatusTypes status = LeadStatusTypes.CallAgainPersonal;
        var ruleService = new MicrosoftRuleEngineProcessingService(
            RuleServiceLoggerMock.Object,
            EngineMapper.Object,
            CDRRepositoryMock.Object,
            EmptySettingsRepositoryMock.Object
        );
        var leads = new List<TrackedLead>();
        var rand = new Random();
        const int firstAgentId = 30;
        const int secondAgentId = 29;
        for (var i = 1; i <= 10000; i++)
        {
            var isExistsAgent = rand.Next(0, 2) > 0;
            var hasAssignedAgent = rand.Next(0, 2) > 0;

            leads.Add(new TrackedLead
                (
                    0,
                    "First_" + i,
                    "Last_" + i,
                    "Phone_" + i,
                    (LeadStatusTypes)rand.Next(1, 29),
                    null,
                    DateTimeOffset.UtcNow, isExistsAgent ? DateTimeOffset.UtcNow : null,
                    lastCallAgentId: isExistsAgent ? firstAgentId : null,
                    null, null, null, false, assignedAgentId: hasAssignedAgent ? secondAgentId : null, null, null,
                    null
                )
            );
        }


        var rules = GetQueueFakeRulesLeadStatusIsCallAgainPersonal_AssignLead(status);

        var countLeadHasAssignedAgentByEngine = leads.Count(x => x.Status == status
                                                                 && x.RemindOn.HasValue
                                                                 && x.LastCallAgentId.HasValue
                                                                 && x.AssignedAgentId == null);
        var countLeadHasAssignedAgentBefore = leads.Count(x => x.AssignedAgentId.HasValue);

        await ruleService.Process(1,
            leads,
            rules,
            RuleGroupTypes.Behavior);


        var countLeadsAfterProcessFirst = leads.Count(x => x.AssignedAgentId == firstAgentId);
        var countLeadsAfterProcessSecond = leads.Count(x => x.AssignedAgentId == secondAgentId);

        Assert.Equal(countLeadHasAssignedAgentByEngine, countLeadsAfterProcessFirst);
        Assert.Equal(countLeadHasAssignedAgentBefore, countLeadsAfterProcessSecond);
    }

    [Theory]
    [InlineData(0, 4, ComparisonOperation.EqualForLastYDays)]
    [InlineData(2, 9, ComparisonOperation.MoreThanForLastYDays)]
    [InlineData(2, null, ComparisonOperation.MoreThan)]
    [InlineData(3, null, ComparisonOperation.Equal)]
    public async Task Process_TotalCallsCount(int calls, int? period, ComparisonOperation operation)
    {
        await using var context = new KlDbContext(DbContextOptions);
        
        var ruleService = new MicrosoftRuleEngineProcessingService(
            RuleServiceLoggerMock.Object,
            EngineMapper.Object,
            new CDRRepository(context),
            EmptySettingsRepositoryMock.Object
        );

        var leads = new List<TrackedLead>();
        var rand = new Random();

        for (var i = 1; i <= 3; i++)
        {
            var item = new TrackedLead
            (
                i,
                "First_" + i,
                "Last_" + i,
                "Phone_" + i,
                (LeadStatusTypes)rand.Next(1, 29),
                null,
                DateTimeOffset.UtcNow,
                null, null, null, null, 
                null, false, null, null, 
                null, null
            )
            {
                Status = LeadStatusTypes.Busy,
            };
            leads.Add(item);
        }

        var rules = GetQueueFakeRulesCampaignLeadTotalCalls(calls, period, operation);

        await ruleService.Process(1,
            leads,
            rules,
            RuleGroupTypes.Behavior);

        var result = leads.First(r => r.LeadId == 1);
        Assert.Equal(LeadStatusTypes.MaxCall, result.Status);
    }

    [Theory]
    [InlineData(1800, ComparisonOperation.Equal)]
    [InlineData(1801, ComparisonOperation.LessThan)]
    [InlineData(1799, ComparisonOperation.MoreThan)]
    public async Task Process_TotalCallsSeconds(int calls, ComparisonOperation operation)
    {
        await using var context = new KlDbContext(DbContextOptions);

        var ruleService = new MicrosoftRuleEngineProcessingService(
            RuleServiceLoggerMock.Object,
            EngineMapper.Object,
            new CDRRepository(context),
            EmptySettingsRepositoryMock.Object
        );

        var leads = new List<TrackedLead>();
        var rand = new Random();
        var status = (LeadStatusTypes)rand.Next(1, 29);
        if (status == LeadStatusTypes.MaxCall)
            status = LeadStatusTypes.Busy;
        for (var i = 1; i <= 100; i++)
        {
            var item = new TrackedLead
            (
                i,
                "First_" + i,
                "Last_" + i,
                "Phone_" + i,
                status,
                null,
                DateTimeOffset.UtcNow,
                null, null, null, null, null, false, null, null, null, null
            );
            leads.Add(item);
        }

        var leadAndCallDurations = GenerateCdRs(leads).GroupBy(x => x.LeadId).SelectMany(x =>
        {
            return new[] { (x.Key!.Value, x.Sum(y => y.CallDuration!.Value)) };
        }).ToDictionary(x => x.Value, x => x.Item2);

        var expectedValue = operation switch
        {
            ComparisonOperation.Equal => leadAndCallDurations.Count(x => x.Value == calls),
            ComparisonOperation.MoreThan => leadAndCallDurations.Count(x => x.Value > calls),
            ComparisonOperation.LessThan => leadAndCallDurations.Count(x => x.Value < calls),
            _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
        };

        var data = GetQueueFakeRulesCampaignLeadCallsSeconds(calls, operation);

        await ruleService.Process(1,
            leads,
            data,
            RuleGroupTypes.Behavior);


        Assert.Equal(expectedValue, leads.Count(x => x.Status == LeadStatusTypes.MaxCall));
    }

    [Theory]
    [InlineData(LeadStatusTypes.CallAgainPersonal, ComparisonOperation.Is)]
    [InlineData(LeadStatusTypes.NewLead, ComparisonOperation.IsNot)]
    [InlineData(LeadStatusTypes.DNC, ComparisonOperation.IsNot)]
    [InlineData(LeadStatusTypes.Busy, ComparisonOperation.IsNot)]
    public async Task Process_PreviousStatus(LeadStatusTypes status, ComparisonOperation operation)
    {
        await using var context = new KlDbContext(DbContextOptions);

        var ruleService = new MicrosoftRuleEngineProcessingService(
            RuleServiceLoggerMock.Object,
            EngineMapper.Object,
            CDRRepositoryMock.Object,
            EmptySettingsRepositoryMock.Object
        );

        var leads = new List<TrackedLead>();
        var rand = new Random();

        for (var i = 1; i <= 3; i++)
        {
            var statusHistoryArray = new List<KeyValuePair<DateTimeOffset, string>>();
            if (i == 1)
            {
                statusHistoryArray.Add(new KeyValuePair<DateTimeOffset, string>(DateTimeOffset.UtcNow.AddDays(-5),
                    JsonSerializer.Serialize(new LeadHistoryChangesDto<object?>
                    {
                        Properties = new List<ValueChanges<object?>>
                            { new("Status", LeadStatusTypes.NewLead, LeadStatusTypes.Busy) }
                    }, SerializerOptions)));
                statusHistoryArray.Add(new KeyValuePair<DateTimeOffset, string>(DateTimeOffset.UtcNow.AddDays(-4),
                    JsonSerializer.Serialize(new LeadHistoryChangesDto<object?>
                    {
                        Properties = new List<ValueChanges<object?>>
                            { new("Status", LeadStatusTypes.Busy, LeadStatusTypes.CallAgainPersonal) }
                    }, SerializerOptions)));
                statusHistoryArray.Add(new KeyValuePair<DateTimeOffset, string>(DateTimeOffset.UtcNow.AddDays(-3),
                    JsonSerializer.Serialize(new LeadHistoryChangesDto<object?>
                    {
                        Properties = new List<ValueChanges<object?>>
                            { new("Status", LeadStatusTypes.CallAgainPersonal, LeadStatusTypes.DNC) }
                    }, SerializerOptions)));
            }

            var item = new TrackedLead
            (
                i,
                "First_" + i,
                "Last_" + i,
                "Phone_" + i,
                (LeadStatusTypes)rand.Next(1, 29),
                null,
                DateTimeOffset.UtcNow,
                null, null, null, null, null, false, null, null,
                statusHistoryArray, null
            )
            {
                Status = LeadStatusTypes.Busy,
            };
            leads.Add(item);
        }

        var rules = GetQueueFakeRulesCampaignLeadPrevStatus(status, operation);

        await ruleService.Process(1,
            leads,
            rules,
            RuleGroupTypes.Behavior);

        var result = leads.First(r => r.LeadId == 1);
        Assert.Equal(LeadStatusTypes.MaxCall, result.Status);
    }

    [Fact]
    public async Task RuleEngine_FewRules_WithSameRulesActions()
    {
        var ruleService = new MicrosoftRuleEngineProcessingService(
            RuleServiceLoggerMock.Object,
            EngineMapper.Object,
            CDRRepositoryMock.Object,
            EmptySettingsRepositoryMock.Object
        );

        var leads = new List<TrackedLead>();

        for (var i = 1; i <= 3; i++)
        {
            var item = new TrackedLead(
                i,
                "First_" + i,
                "Last_" + i,
                "Phone_" + i,
                LeadStatusTypes.NewLead,
                null,
                DateTimeOffset.UtcNow,
                null, null, null, null, null, false, null, null, null, null)
            {
                Score = i != 1 ? 50000 : 100,
            };
            leads.Add(item);
        }

        var rules = GetQueueFakeRulesWithSameRulesActions();

        await ruleService.Process(1,
            leads,
            rules,
            RuleGroupTypes.Behavior);

        Assert.True(true);
    }

    [Fact]
    public async Task RuleEngine_FewRules_WithSameRulesConditions()
    {
        var ruleService = new MicrosoftRuleEngineProcessingService(
            RuleServiceLoggerMock.Object,
            EngineMapper.Object,
            CDRRepositoryMock.Object,
            EmptySettingsRepositoryMock.Object
        );

        var leads = new List<TrackedLead>();

        for (var i = 1; i <= 3; i++)
        {
            var item = new TrackedLead(
                i,
                "First_" + i,
                "Last_" + i,
                "Phone_" + i,
                LeadStatusTypes.NewLead,
                null,
                DateTimeOffset.UtcNow,
                null, null, null, null, null, false, null, null, null, null)
            {
                Score = i != 1 ? 50000 : 100,
            };
            leads.Add(item);
        }

        var rules = GetQueueFakeRulesWithSameRulesConditions();

        await ruleService.Process(1,
            leads,
            rules,
            RuleGroupTypes.Behavior);


        Assert.True(true);
    }

    [Fact]
    public async Task RuleEngine_FutureCall_Multiple()
    {
        var ruleService = new MicrosoftRuleEngineProcessingService(
            RuleServiceLoggerMock.Object,
            EngineMapper.Object,
            CDRRepositoryMock.Object,
            EmptySettingsRepositoryMock.Object
        );

        var leads = new List<TrackedLead>();
        var dateTime = DateTimeOffset.UtcNow;
        var endOfDay = new DateTimeOffset(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59, TimeSpan.Zero);
        //var endOfDay2 = DateTimeOffset.UtcNow.EndOfDay();

        for (var i = 1; i <= 3; i++)
        {
            var item = new TrackedLead(
                i,
                "First_" + i,
                "Last_" + i,
                "Phone_" + i,
                i % 2 == 0 ? LeadStatusTypes.CallAgainPersonal : LeadStatusTypes.CallAgainGeneral,
                null,
                DateTimeOffset.UtcNow,
                i == 2 ? endOfDay : DateTimeOffset.UtcNow.AddDays(i - 1),
                i == 1 ? null : i,
                null, null, null, false, null, null, null, null);
            leads.Add(item);
        }

        var rules = GetFutureCallMultipleFakeRules();

        await ruleService.Process(1,
            leads,
            rules,
            RuleGroupTypes.ForwardRules);

        Assert.True(leads.First(r => r.LeadId == 1).LeadQueueId == 11);
        Assert.True(leads.First(r => r.LeadId == 2).LeadQueueId == 11);
        Assert.True(leads.First(r => r.LeadId == 3).LeadQueueId == null);
    }


    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task RuleEngine_CurrentStatus(bool toggle)
    {
        var ruleService = new MicrosoftRuleEngineProcessingService(
            RuleServiceLoggerMock.Object,
            EngineMapper.Object,
            CDRRepositoryMock.Object,
            EmptySettingsRepositoryMock.Object
        );

        var leads = new List<TrackedLead>();
        var dateTime = DateTimeOffset.UtcNow;
        var endOfDay = new DateTimeOffset(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59, TimeSpan.Zero);

        for (var i = 1; i <= 6; i++)
        {
            var item = new TrackedLead(
                i,
                "First_" + i,
                "Last_" + i,
                "Phone_" + i,
                i % 2 == 0 && toggle ? LeadStatusTypes.CallAgainPersonal : LeadStatusTypes.CallAgainGeneral,
                null,
                DateTimeOffset.UtcNow,
                i == 2 ? endOfDay : DateTimeOffset.UtcNow.AddDays(i - 1),
                i == 1 ? null : i,
                null, null, null, false, null, null, null, null);
            leads.Add(item);
        }

        var rules = GetCurrentStatusFakeRules();

        await ruleService.Process(1,
            leads,
            rules,
            RuleGroupTypes.ForwardRules);

        if (toggle)
        {
            Assert.True(3 == leads.Count(r => r.LeadQueueId == 11));
        }
        else
        {
            Assert.True(0 == leads.Count(r => r.LeadQueueId == 11));
        }
    }

    [Fact]
    public async Task RuleEngine_ClientsMetaConditionsIdentifier()
    {
        var ruleService = new MicrosoftRuleEngineProcessingService(
            RuleServiceLoggerMock.Object,
            EngineMapper.Object,
            CDRRepositoryMock.Object,
            SettingsRepositoryMock.Object
        );

        var leads = new List<TrackedLead>();

        for (var i = 1; i <= 10; i++)
        {
            var item = new TrackedLead(
                i,
                "First_" + i,
                "Last_" + i,
                "Phone_" + i,
                LeadStatusTypes.NewLead,
                null,
                DateTimeOffset.UtcNow,
                null, null, null, null, null, false, null, null, null,
                $"{{\"campaign_id\":\"{i % 3}\"}}"
            )
            {
                LeadQueueId = 0
            };
            leads.Add(item);
        }

        var rules = GetQueueFakeRulesLeadFieldCampaignId();

        await ruleService.Process(1,
            leads,
            rules,
            RuleGroupTypes.ForwardRules);

        Assert.Equal(4, leads.Count(r => r.LeadQueueId == 11));
    }

    [Fact]
    public async Task RuleEngine_ClientsMetaConditionsInteger()
    {
        var ruleService = new MicrosoftRuleEngineProcessingService(
            RuleServiceLoggerMock.Object,
            EngineMapper.Object,
            CDRRepositoryMock.Object,
            SettingsRepositoryMock.Object
        );

        var leads = new List<TrackedLead>();

        for (var i = 1; i <= 10; i++)
        {
            var item = new TrackedLead(
                i,
                "First_" + i,
                "Last_" + i,
                "Phone_" + i,
                LeadStatusTypes.NewLead,
                null,
                DateTimeOffset.UtcNow,
                null, null, null, null, null, false, null, null, null,
                $"{{\"score\": {i * 30}}}"
            )
            {
                LeadQueueId = 0
            };
            leads.Add(item);
        }

        var rules = GetQueueFakeRulesLeadFieldScore();

        await ruleService.Process(1,
            leads,
            rules,
            RuleGroupTypes.ForwardRules);

        Assert.Equal(5, leads.Count(r => r.LeadQueueId == 11));
    }


    [Fact]
    public async Task RuleEngine_ClientsMetaConditionsString()
    {
        var ruleService = new MicrosoftRuleEngineProcessingService(
            RuleServiceLoggerMock.Object,
            EngineMapper.Object,
            CDRRepositoryMock.Object,
            SettingsRepositoryMock.Object
        );

        var leads = new List<TrackedLead>();

        for (var i = 1; i <= 10; i++)
        {
            var testStr = i % 3 == 0 ? "!_test_!" : "";
            var item = new TrackedLead(
                i,
                "First_" + i,
                "Last_" + i,
                "Phone_" + i,
                LeadStatusTypes.NewLead,
                null,
                DateTimeOffset.UtcNow,
                null, null, null, null, null, false, null, null, null,
                $"{{\"campaign_name\": \"{i * 12333123}_{testStr}\"}}"
            )
            {
                LeadQueueId = 0
            };
            leads.Add(item);
        }

        var rules = GetQueueFakeRulesLeadFieldCampaignName();

        await ruleService.Process(1,
            leads,
            rules,
            RuleGroupTypes.ForwardRules);

        Assert.Equal(3, leads.Count(r => r.LeadQueueId == 11));
    }

    [Fact]
    public async Task RuleEngine_ClientsMetaConditionsIntegerSet()
    {
        var ruleService = new MicrosoftRuleEngineProcessingService(
            RuleServiceLoggerMock.Object,
            EngineMapper.Object,
            CDRRepositoryMock.Object,
            SettingsRepositoryMock.Object
        );

        var leads = new List<TrackedLead>();

        var ids = "1";
        for (var i = 1; i <= 10; i++)
        {
            var item = new TrackedLead(
                i,
                "First_" + i,
                "Last_" + i,
                "Phone_" + i,
                LeadStatusTypes.NewLead,
                null,
                DateTimeOffset.UtcNow,
                null, null, null, null, null, false, null, null, null,
                $"{{\"group_ids\": [{ids}]}}"
            )
            {
                LeadQueueId = 0
            };
            leads.Add(item);
            ids = ids + $",{i + 1}";
        }

        var rules = GetQueueFakeRulesLeadFieldGroupIdsContains();

        await ruleService.Process(1,
            leads,
            rules,
            RuleGroupTypes.ForwardRules);

        Assert.Equal(5, leads.Count(r => r.LeadQueueId == 11));

        rules = GetQueueFakeRulesLeadFieldGroupIdsEqual();
        leads.ForEach(x => x.LeadQueueId = 0);

        await ruleService.Process(1,
            leads,
            rules,
            RuleGroupTypes.ForwardRules);

        Assert.Equal(1, leads.Count(r => r.LeadQueueId == 11));
    }


    [Fact]
    public async Task RuleEngine_ClientsMetaConditionsStringSet()
    {
        var ruleService = new MicrosoftRuleEngineProcessingService(
            RuleServiceLoggerMock.Object,
            EngineMapper.Object,
            CDRRepositoryMock.Object,
            SettingsRepositoryMock.Object
        );

        var leads = new List<TrackedLead>();

        var types = "\"test1\"";
        for (var i = 1; i <= 10; i++)
        {
            var item = new TrackedLead(
                i,
                "First_" + i,
                "Last_" + i,
                "Phone_" + i,
                LeadStatusTypes.NewLead,
                null,
                DateTimeOffset.UtcNow,
                null, null, null, null, null, false, null, null, null,
                $"{{\"campaign_type\": [{types}]}}"
            )
            {
                LeadQueueId = 0
            };
            leads.Add(item);
            types = types + $",\"test{i + 1}\"";
        }

        var rules = GetQueueFakeRulesLeadFieldCampaignTypeContains();

        await ruleService.Process(1,
            leads,
            rules,
            RuleGroupTypes.ForwardRules);

        Assert.Equal(3, leads.Count(r => r.LeadQueueId == 11));
    }

    [Fact]
    public async Task NewStatus_is_CheckNumber_Then_Delete_Lead()
    {
        var leads = new[]
        {
            GetLead_Default(status: LeadStatusTypes.CheckNumber),
            GetLead_Default(status: LeadStatusTypes.CheckNumber),
            GetLead_Default(status: LeadStatusTypes.NotInterested),
            GetLead_Default(status: LeadStatusTypes.NotInterested),
        };

        var newStatusCondition = NewStatus_Condition(ComparisonOperation.Is, LeadStatusTypes.CheckNumber);
        var deleteAction = DeleteLead_Action();
        var rules = Combine(newStatusCondition, RuleCombinationOperator.None, deleteAction);

        await RuleEngine.Process(1, leads, rules, RuleGroupTypes.Behavior);

        Assert.NotNull(leads[0].DeletedOn);
        Assert.NotNull(leads[1].DeletedOn);
        Assert.Null(leads[2].DeletedOn);
        Assert.Null(leads[3].DeletedOn);
    }

    [Fact]
    public async Task NewStatusIs_and_NewStatusWasTotalXTimes_Then_FreezeLeadForX()
    {
        const int countChangesStatus = 4;
        const int countHours = 24;
        var history = PrepareLeadStatusHistory(LeadStatusTypes.SmallBarrier, countChangesStatus);
        var leads = new[]
        {
            GetLead_Default(status: LeadStatusTypes.CheckNumber),
            GetLead_Default(status: LeadStatusTypes.CheckNumber),
            GetLead_Default(status: LeadStatusTypes.SmallBarrier, statusHistory: history),
            GetLead_Default(status: LeadStatusTypes.SmallBarrier, statusHistory: history),
        };

        var newStatusCondition = NewStatus_Condition(ComparisonOperation.Is, LeadStatusTypes.SmallBarrier);
        var newStatusWasTotalCondition = NewStatusWasTotalXTimes_Condition(ComparisonOperation.LessThan, 5);

        var freezeAction = FreezeLeadForX_Action(countHours, TimeUnits.Hours);

        var rules = Combine(new[]
        {
            newStatusCondition, newStatusWasTotalCondition
        }, RuleCombinationOperator.And, freezeAction);

        await RuleEngine.Process(1, leads, rules, RuleGroupTypes.Behavior);

        Assert.Null(leads[0].FreezeTo);
        Assert.Null(leads[1].FreezeTo);
        Assert.True(leads[2].FreezeTo <= DateTimeOffset.UtcNow + TimeSpan.FromHours(countHours));
        Assert.True(leads[3].FreezeTo <= DateTimeOffset.UtcNow + TimeSpan.FromHours(countHours));
    }

    [Fact]
    public async Task NewStatusIs_and_CampaignLeadWeightIs_NewStatusWasTotal_Then_FreezeLeadForX()
    {
        const int countChangesStatus = 4;
        const int countMinutesFreeze = 10;
        var history = PrepareLeadStatusHistory(LeadStatusTypes.NA, countChangesStatus);
        var leads = new[]
        {
            GetLead_Default(status: LeadStatusTypes.CheckNumber),
            GetLead_Default(status: LeadStatusTypes.CheckNumber),
            GetLead_Default(status: LeadStatusTypes.NA, statusHistory: history, score: 2500),
            GetLead_Default(status: LeadStatusTypes.NA, statusHistory: history, score: 3000),
        };

        var newStatusCondition = NewStatus_Condition(ComparisonOperation.Is, LeadStatusTypes.NA);
        var campaignLeadWeight = CampaignLeadWeightIs_Condition(ComparisonOperation.MoreThan, 1200);
        var newStatusWasTotalCondition = NewStatusWasTotalXTimes_Condition(ComparisonOperation.LessThan, 5);
        var campaignLeadWeight2 = CampaignLeadWeightIs_Condition(ComparisonOperation.LessThan, 9999);

        var freezeAction = FreezeLeadForX_Action(countMinutesFreeze, TimeUnits.Minutes);

        var rules = Combine(new[]
        {
            newStatusCondition, campaignLeadWeight,
            newStatusWasTotalCondition, campaignLeadWeight2
        }, RuleCombinationOperator.And, freezeAction);

        await RuleEngine.Process(1, leads, rules, RuleGroupTypes.Behavior);

        Assert.Null(leads[0].FreezeTo);
        Assert.Null(leads[1].FreezeTo);
        Assert.True(leads[2].FreezeTo <= DateTimeOffset.UtcNow + TimeSpan.FromMinutes(countMinutesFreeze));
        Assert.True(leads[3].FreezeTo <= DateTimeOffset.UtcNow + TimeSpan.FromMinutes(countMinutesFreeze));
    }

    [Fact]
    public async Task NewStatusIs_and_CampaignLeadWeightIs_NewStatusWasTotalXPeriod_Then_FreezeLeadForX()
    {
        const int countChangesStatus = 4;
        const int countMinutesFreeze = 130;
        var history = PrepareLeadStatusHistory(LeadStatusTypes.SystemFailedToConnect, countChangesStatus);
        var leads = new[]
        {
            GetLead_Default(status: LeadStatusTypes.CheckNumber),
            GetLead_Default(status: LeadStatusTypes.CheckNumber),
            GetLead_Default(status: LeadStatusTypes.SystemFailedToConnect, statusHistory: history, score: 500),
            GetLead_Default(status: LeadStatusTypes.SystemFailedToConnect, statusHistory: history, score: 1000),
        };

        var newStatusCondition = NewStatus_Condition(ComparisonOperation.Is, LeadStatusTypes.SystemFailedToConnect);
        var campaignLeadWeight = CampaignLeadWeightIs_Condition(ComparisonOperation.LessThan, 1199);
        var newStatusWasTotalXTimesForYPeriodCondition =
            NewStatusWasTotalXTimesYPeriod_Condition(ComparisonOperation.MoreThanForLastYHours, 1, 12);

        var freezeAction = FreezeLeadForX_Action(countMinutesFreeze, TimeUnits.Minutes);

        var rules = Combine(new[]
        {
            newStatusCondition,
            newStatusWasTotalXTimesForYPeriodCondition, campaignLeadWeight,
        }, RuleCombinationOperator.And, freezeAction);

        await RuleEngine.Process(1, leads, rules, RuleGroupTypes.Behavior);

        Assert.Null(leads[0].FreezeTo);
        Assert.Null(leads[1].FreezeTo);
        Assert.True(leads[2].FreezeTo <= DateTimeOffset.UtcNow + TimeSpan.FromMinutes(countMinutesFreeze));
        Assert.True(leads[3].FreezeTo <= DateTimeOffset.UtcNow + TimeSpan.FromMinutes(countMinutesFreeze));
    }

    [Fact]
    public async Task CountryIs_LeadIsInSystem_CurrentStatusIsNot_Then_MoveLeadToQueue()
    {
        const long leadQueueId = 10;
        var leads = new[]
        {
            GetLead_Default(countryCode: "UA", registrationTime: DateTimeOffset.UtcNow),
            GetLead_Default(countryCode: "UA"),
            GetLead_Default(countryCode: "SE", registrationTime: DateTimeOffset.UtcNow),
            GetLead_Default(countryCode: "SE", registrationTime: DateTimeOffset.UtcNow, status: LeadStatusTypes.Busy),
            GetLead_Default(countryCode: "SE", registrationTime: DateTimeOffset.UtcNow.AddHours(-2),
                status: LeadStatusTypes.DNC),
            GetLead_Default(countryCode: "SE", registrationTime: DateTimeOffset.UtcNow.AddHours(-2),
                status: LeadStatusTypes.DNCCountry),
            GetLead_Default(countryCode: "SE", registrationTime: DateTimeOffset.UtcNow.AddHours(-2),
                status: LeadStatusTypes.Busy),
        };

        var countryCondition = Country_Condition(ComparisonOperation.Is, "SE");
        var leadInSystemCondition = LeadIsInTheSystem_Condition(ComparisonOperation.MoreThan, 30, TimeUnits.Minutes);
        var leadInSystemCondition2 = LeadIsInTheSystem_Condition(ComparisonOperation.LessThan, 60, TimeUnits.Days);
        var currentStatusCondition = CurrentStatus_Condition(
            ComparisonOperation.IsNot,
            LeadStatusTypes.DNC,
            LeadStatusTypes.DNCCountry,
            LeadStatusTypes.NotInterested);


        var moveLeadToQueueAction = MoveLeadToQueue_Action(10);

        var rules = Combine(new[]
        {
            countryCondition, leadInSystemCondition,
            leadInSystemCondition2, currentStatusCondition
        }, RuleCombinationOperator.And, moveLeadToQueueAction);

        await RuleEngine.Process(1, leads, rules, RuleGroupTypes.ForwardRules);


        Assert.All(leads.SkipLast(1), x => Assert.False(x.LeadQueueId == leadQueueId));
        Assert.True(leads.Last().LeadQueueId == leadQueueId);
    }

    [Fact]
    public async Task CountryIs_Then_SetLeadWeightByX()
    {
        var leads = new[]
        {
            GetLead_Default(countryCode: "SE", score: 25),
            GetLead_Default(countryCode: "UA", score: 75),
        };

        var countryCondition = Country_Condition(ComparisonOperation.Is, "UA");

        var setLeadWeightByXAction = SetLeadWeightByX_Action(ActionOperation.Decrease, 25);

        var rules = Combine(new[]
        {
            countryCondition,
        }, RuleCombinationOperator.And, setLeadWeightByXAction);

        await RuleEngine.Process(1, leads, rules, RuleGroupTypes.LeadScoring);

        Assert.Equal(25, leads[0].Score);
        Assert.Equal(50, leads[1].Score);
    }

    [Fact]
    public async Task NewStatusWasTotalXTimes_Then_SetLeadWeightByX()
    {
        const int countStatusChanged = 4;
        var history = PrepareLeadStatusHistory(LeadStatusTypes.NewLead, countStatusChanged);
        var leads = new[]
        {
            GetLead_Default(status: LeadStatusTypes.Busy, score: 25),
            GetLead_Default(status: LeadStatusTypes.NewLead, statusHistory: history, score: 75),
        };

        var newStatusWasTotalXTimesCondition = NewStatusWasTotalXTimes_Condition(ComparisonOperation.MoreThan, 2);
        var setLeadWeightByXAction = SetLeadWeightByX_Action(ActionOperation.Decrease, 25);

        var rules = Combine(new[]
        {
            newStatusWasTotalXTimesCondition,
        }, RuleCombinationOperator.And, setLeadWeightByXAction);

        await RuleEngine.Process(1, leads, rules, RuleGroupTypes.LeadScoring);

        Assert.Equal(25, leads[0].Score);
        Assert.Equal(50, leads[1].Score);
    }

    [Fact]
    public async Task NewStatusIs_NewStatusWasTotalXTimes_Then_SetLeadWeightByX()
    {
        const int countStatusChanged = 2;
        var history = PrepareLeadStatusHistory(LeadStatusTypes.NoMoney, countStatusChanged);
        var leads = new[]
        {
            GetLead_Default(score: 25),
            GetLead_Default(status: LeadStatusTypes.NoMoney, statusHistory: history, score: 75),
        };

        var newStatusCondition = NewStatus_Condition(ComparisonOperation.Is, LeadStatusTypes.NoMoney);
        var newStatusWasTotalXTimesCondition = NewStatusWasTotalXTimes_Condition(ComparisonOperation.LessThan, 3);
        var setLeadWeightByXAction = SetLeadWeightByX_Action(ActionOperation.Decrease, 25);

        var rules = Combine(new[]
        {
            newStatusCondition,
            newStatusWasTotalXTimesCondition,
        }, RuleCombinationOperator.And, setLeadWeightByXAction);

        await RuleEngine.Process(1, leads, rules, RuleGroupTypes.LeadScoring);

        Assert.Equal(25, leads[0].Score);
        Assert.Equal(50, leads[1].Score);
    }

    [Fact]
    public async Task CountryIs_Then_ChangeStatus()
    {
        var leads = new[]
        {
            GetLead_Default(countryCode: "PL"),
            GetLead_Default(countryCode: "HZ"),
            GetLead_Default(countryCode: "UA"),
            GetLead_Default(countryCode: "SA"),
        };

        var countryCondition = Country_Condition(ComparisonOperation.Is, "UA", "SA", "GE");
        var changeStatusCondition = ChangeStatus_Action(LeadStatusTypes.DNCCountry);

        var rules = Combine(new[]
        {
            countryCondition,
        }, RuleCombinationOperator.And, changeStatusCondition);

        await RuleEngine.Process(1, leads, rules, RuleGroupTypes.ApiRules);

        Assert.Equal(LeadStatusTypes.NewLead, leads[0].Status);
        Assert.Equal(LeadStatusTypes.NewLead, leads[1].Status);
        Assert.Equal(LeadStatusTypes.DNCCountry, leads[2].Status);
        Assert.Equal(LeadStatusTypes.DNCCountry, leads[3].Status);
    }

    [Fact]
    public async Task CurrentStatusIs_IsFutureCall_Then_MoveLeadToQueue()
    {
        const int leadQueueId = 10;
        var leads = new[]
        {
            GetLead_Default(status: LeadStatusTypes.Busy),
            GetLead_Default(status: LeadStatusTypes.CallAgainPersonal),
            GetLead_Default(status: LeadStatusTypes.CallAgainGeneral),
            GetLead_Default(status: LeadStatusTypes.CallAgainGeneral, remindOn: DateTimeOffset.UtcNow.AddDays(-1)),
            GetLead_Default(status: LeadStatusTypes.CallAgainPersonal, remindOn: DateTimeOffset.UtcNow.AddDays(-1)),
        };

        var countryCondition = CurrentStatus_Condition(ComparisonOperation.Is,
            LeadStatusTypes.CallAgainPersonal, LeadStatusTypes.CallAgainGeneral);
        var isFutureCallCondition = IsFutureCall_Condition();
        var moveLeadToQueueAction = MoveLeadToQueue_Action(leadQueueId);

        var rules = Combine(new[]
        {
            countryCondition,
            isFutureCallCondition,
        }, RuleCombinationOperator.And, moveLeadToQueueAction);

        await RuleEngine.Process(1, leads, rules, RuleGroupTypes.ForwardRules);

        Assert.Null(leads[0].LeadQueueId);
        Assert.Null(leads[1].LeadQueueId);
        Assert.Null(leads[2].LeadQueueId);
        Assert.Equal(leadQueueId, leads[3].LeadQueueId);
        Assert.Equal(leadQueueId, leads[4].LeadQueueId);
    }

    [Fact]
    public async Task CountryIs_CurrentStatusIs_IsTestLead_Then_FreezeLeadForX_DeleteLead()
    {
        const int countFreezeMinutes = 15;
        var leads = new[]
        {
            GetLead_Default(),
            GetLead_Default(countryCode: "UA"),
            GetLead_Default(countryCode: "PL", isTest: false),
            GetLead_Default(countryCode: "PL", isTest: true),
        };

        var countryCondition = Country_Condition(ComparisonOperation.Is, "UA", "PL");
        var isTestLeadCondition = IsTestLead_Condition(ComparisonOperation.Is);

        var setLeadWeightByXAction = FreezeLeadForX_Action(countFreezeMinutes, TimeUnits.Minutes);
        var deleteLeadAction = DeleteLead_Action();

        var rules = Combine(new[]
        {
            countryCondition, isTestLeadCondition
        }, RuleCombinationOperator.And, setLeadWeightByXAction, deleteLeadAction);

        await RuleEngine.Process(1, leads, rules, RuleGroupTypes.Behavior);

        Assert.All(leads.SkipLast(1), x => Assert.Null(x.FreezeTo));
        Assert.All(leads.SkipLast(1), x => Assert.Null(x.DeletedOn));
        Assert.True(leads[3].FreezeTo <= DateTimeOffset.UtcNow.AddMinutes(countFreezeMinutes));
        Assert.True(leads[3].DeletedOn <= DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task IsImported_IsTest_Then_MoveToLeadQueue()
    {
        const long leadQueueId = 10;
        var leads = new[]
        {
            GetLead_Default(systemStatus: LeadSystemStatusTypes.Processing, isTest: false),
            GetLead_Default(systemStatus: LeadSystemStatusTypes.Processing, isTest: true),
            GetLead_Default(systemStatus: LeadSystemStatusTypes.Imported, isTest: false),
            GetLead_Default(systemStatus: LeadSystemStatusTypes.Imported, isTest: true),
        };

        var isImportedCondition = IsImported_Condition();
        var isTestLeadCondition = IsTestLead_Condition(ComparisonOperation.Is);

        var setLeadWeightByXAction = MoveLeadToQueue_Action(leadQueueId);

        var rules = Combine(new[]
        {
            isImportedCondition, isTestLeadCondition
        }, RuleCombinationOperator.And, setLeadWeightByXAction);

        await RuleEngine.Process(1, leads, rules, RuleGroupTypes.ForwardRules);

        Assert.Null(leads[0].LeadQueueId);
        Assert.Null(leads[1].LeadQueueId);
        Assert.Null(leads[2].LeadQueueId);
        Assert.Equal(10, leads[3].LeadQueueId);
    }

    [Fact]
    public async Task PreviousStatus_NewStatus_Then_DeleteLead()
    {
        var history = PrepareLeadStatusHistory(LeadStatusTypes.WrongNumber, 3);
        var leads = new[]
        {
            GetLead_Default(status: LeadStatusTypes.Busy),
            GetLead_Default(status: LeadStatusTypes.Busy, statusHistory: history),
            GetLead_Default(status: LeadStatusTypes.WrongNumber),
            GetLead_Default(status: LeadStatusTypes.WrongNumber, statusHistory: history),
        };

        var previousStatusCondition = PreviousStatus_Condition(ComparisonOperation.Is, LeadStatusTypes.WrongNumber);
        var newStatusCondition = NewStatus_Condition(ComparisonOperation.Is, LeadStatusTypes.WrongNumber);

        var deleteLeadAction = DeleteLead_Action();

        var rules = Combine(new[]
        {
            previousStatusCondition, newStatusCondition
        }, RuleCombinationOperator.And, deleteLeadAction);

        await RuleEngine.Process(1, leads, rules, RuleGroupTypes.Behavior);

        Assert.All(leads.SkipLast(1), x => Assert.Null(x.DeletedOn));
        Assert.True(leads[3].DeletedOn <= DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task NewStatusIs_Then_All_Actions()
    {
        var leads = new[]
        {
            GetLead_Default(lastCallAgentId: 30, status: LeadStatusTypes.Duplicate)
        };

        var newStatusCondition = NewStatus_Condition(ComparisonOperation.Is, LeadStatusTypes.Duplicate);

        var assignAgentToLeadAction = AssignAgent_Action();
        var changeLeadFieldsValueAction = ChangeLeadFieldsValue_Action(nameof(TrackedLead.FirstName), "John");
        var changeStatusAction = ChangeStatus_Action(LeadStatusTypes.CannotTalk);
        var freezeLeadForXAction = FreezeLeadForX_Action(14, TimeUnits.Days);
        var deleteLeadAction = DeleteLead_Action();

        var rules = Combine(new[]
            {
                newStatusCondition,
            }, RuleCombinationOperator.And,
            assignAgentToLeadAction,
            changeLeadFieldsValueAction,
            changeStatusAction,
            freezeLeadForXAction,
            deleteLeadAction
        );

        await RuleEngine.Process(1, leads, rules, RuleGroupTypes.Behavior);

        Assert.Equal(30, leads[0].AssignedAgentId);
        Assert.Equal("John", leads[0].FirstName);
        Assert.Equal(LeadStatusTypes.CannotTalk, leads[0].Status);
        Assert.True(leads[0].FreezeTo <= DateTimeOffset.UtcNow.AddDays(14));
        Assert.NotNull(leads[0].DeletedOn);
    }
}