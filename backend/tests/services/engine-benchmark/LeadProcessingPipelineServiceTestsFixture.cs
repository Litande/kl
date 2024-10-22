using System.Text.Json;
using Microsoft.Extensions.Logging;
using Moq;

namespace KL.Engine.Benchmark.Tests;

public class LeadProcessingPipelineServiceTestsFixture
{
    private const string InputName = "lead";
    protected readonly Mock<ILogger<MicrosoftRuleEngineProcessingService>> RuleServiceLoggerMock;
    protected readonly Mock<ILeadsQueueStore> LeadsQueueStore;

    public LeadProcessingPipelineServiceTestsFixture()
    {
        RuleServiceLoggerMock = new Mock<ILogger<MicrosoftRuleEngineProcessingService>>();
        LeadsQueueStore = new Mock<ILeadsQueueStore>();
    }

    protected static List<RuleDto> GetQueueFakeRulesPreviousStatusAndChangeStatus()
    {
        var ruleEntry = new RuleEntry()
        {
            Combination = new RuleCombinationData
            {
                Operator = RuleCombinationOperator.None,
                Groups = new List<RuleGroupData>()
                {
                    new RuleGroupData()
                    {
                        Name = RulesCondition.PreviousStatus,
                        Fields = new List<RuleValueData>
                        {
                            new RuleValueData()
                            {
                                Type = RuleValueType.String,
                                Value = LeadStatusTypes.Busy.ToString()
                            }
                        }
                    }
                }
            },
            Actions = new List<RuleActionData>
            {
                new RuleActionData()
                {
                    Name = RulesAction.ChangeStatus.ToString(),
                    ActionOperation = ActionOperation.To,
                    Fields = new List<RuleValueData>
                    {
                        new RuleValueData()
                        {
                            Type = RuleValueType.String,
                            Value = LeadStatusTypes.DNC.ToString()
                        }
                    }
                }
            }
        };

        return new List<RuleDto>
        {
            new(null, "QueueRules", JsonSerializer.Serialize(ruleEntry), 0),
        };
    }

    protected static List<RuleDto> GetQueueFakeRules70PrcEasyAnd30PrcExt(int totalCount = 50)
    {
        var rules = new List<Rule>();
        var rand = new Random();

        for (var i = 0; i < totalCount / 3; i++)
        {
            rules.Add(new Rule
            {
                RuleName = "TestGetQueueIdExt_" + i,
                Operator = "And",
                Rules = new[]
                {
                    new Rule
                    {
                        RuleName = "TestSubGetQueueId_1_" + i,
                        Expression = $"{InputName}.Status == \"{(LeadStatusTypes)rand.Next(1, 29)}\"",
                    },
                    new Rule
                    {
                        RuleName = "TestSubGetQueueId_2_" + i,
                        Expression = $"{InputName}.Score > {rand.Next(100, 10000)}",
                    }
                },
                Actions = new RuleActions
                {
                    OnSuccess = new ActionInfo
                    {
                        Name = "OutputExpression",
                        Context = new Dictionary<string, object> { { "Expression", rand.Next(1, 98) } },
                    }
                },
            });
        }

        for (var i = 0; i < totalCount / 3 * 2; i++)
        {
            rules.Add(new Rule
            {
                RuleName = "TestGetQueueId_" + i,
                Expression = $"{InputName}.Status == \"{(LeadStatusTypes)rand.Next(1, 29)}\"",
                Actions = new RuleActions
                {
                    OnSuccess = new ActionInfo
                    {
                        Name = "OutputExpression",
                        Context = new Dictionary<string, object> { { "Expression", rand.Next(1, 98) } },
                    }
                },
            });
        }

        rules.Add(new Rule
        {
            RuleName = "TestGetDefaultQueueId",
            Expression = "true",
            Actions = new RuleActions
            {
                OnSuccess = new ActionInfo
                {
                    Name = "OutputExpression",
                    Context = new Dictionary<string, object> { { "Expression", 99 } },
                }
            },
        });

        return new List<RuleDto>
        {
            new(null, "QueueRules", JsonSerializer.Serialize(rules), 0),
        };
    }

    protected static List<RuleDto> GetScoreFakeRules70PrcEasyAnd30PrcExt(int totalCount = 50)
    {
        var rules = new List<Rule>();
        var rand = new Random();

        for (var i = 0; i < totalCount / 3; i++)
        {
            rules.Add(new Rule
            {
                RuleName = "TestGetScoreExt_" + i,
                Operator = "And",
                Rules = new[]
                {
                    new Rule
                    {
                        RuleName = "TestSubGetScoreId_1_" + i,
                        Expression = $"{InputName}.Status == \"{(LeadStatusTypes)rand.Next(1, 29)}\"",
                    },
                    new Rule
                    {
                        RuleName = "TestSubGetScoreId_2_" + i,
                        Expression = $"{InputName}.MinutesSinceRegistration <= {rand.Next(1, 50)} AND {InputName}.MinutesSinceRegistration > 0",
                    }
                },
                Actions = new RuleActions
                {
                    OnSuccess = new ActionInfo
                    {
                        Name = "OutputExpression",
                        Context = new Dictionary<string, object> { { "Expression", rand.Next(10000, 100000) } },
                    }
                },
            });
        }

        for (var i = 0; i < totalCount / 3 * 2; i++)
        {
            rules.Add(new Rule
            {
                RuleName = "TestGetScoreId_" + i,
                Expression = $"{InputName}.MinutesSinceRegistration <= {rand.Next(1, 50)} AND {InputName}.MinutesSinceRegistration > 0",
                Actions = new RuleActions
                {
                    OnSuccess = new ActionInfo
                    {
                        Name = "OutputExpression",
                        Context = new Dictionary<string, object> { { "Expression", rand.Next(500, 10000) } },
                    }
                },
            });
        }

        rules.Add(new Rule
        {
            RuleName = "TestGetDefaultScore",
            Expression = "true",
            Actions = new RuleActions
            {
                OnSuccess = new ActionInfo
                {
                    Name = "OutputExpression",
                    Context = new Dictionary<string, object> { { "Expression", 1000 } },
                }
            },
        });

        return new List<RuleDto>
        {
            new(null, "ScoreRules", JsonSerializer.Serialize(rules), 0),
        };
    }
}
