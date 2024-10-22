using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace KL.Engine.Rule.Tests;

public class LeadProcessingPipelineServiceTestsFixture : TestBase
{
    protected readonly Mock<ILogger<MicrosoftEngineMapper>> RuleMapperLoggerMock;
    protected readonly Mock<ILogger<MicrosoftRuleEngineProcessingService>> RuleServiceLoggerMock;
    protected readonly DbContextOptions<DialDbContext> DbContextOptions;
    protected readonly MicrosoftRuleEngineProcessingService RuleEngine;
    protected readonly Mock<MicrosoftEngineMapper> EngineMapper;

    protected readonly Mock<ICDRRepository> CDRRepositoryMock;
    protected readonly Mock<ISettingsRepository> EmptySettingsRepositoryMock;
    protected readonly Mock<ISettingsRepository> SettingsRepositoryMock;
    protected readonly JsonSerializerOptions SerializerOptions;

    public LeadProcessingPipelineServiceTestsFixture()
    {
        RuleMapperLoggerMock = new Mock<ILogger<MicrosoftEngineMapper>>();
        RuleServiceLoggerMock = new Mock<ILogger<MicrosoftRuleEngineProcessingService>>();
        CDRRepositoryMock = new Mock<ICDRRepository>();
        SettingsRepositoryMock = new Mock<ISettingsRepository>();
        EmptySettingsRepositoryMock = new Mock<ISettingsRepository>();
        EngineMapper = new Mock<MicrosoftEngineMapper>(RuleMapperLoggerMock.Object);

        DbContextOptions = new DbContextOptionsBuilder<DialDbContext>()
            .UseInMemoryDatabase(databaseName: "DialDbContextInMemory")
            .Options;

        SettingsRepositoryMock.Setup(r => r.GetValue(It.IsAny<long>(), It.IsAny<SettingTypes>(), default))
            .Returns(GetFakeRuleSettings());

        SerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };
        RuleEngine = new MicrosoftRuleEngineProcessingService(
            RuleServiceLoggerMock.Object,
            new MicrosoftEngineMapper(RuleMapperLoggerMock.Object), CDRRepositoryMock.Object, SettingsRepositoryMock.Object);
    }

    #region Rules

    protected List<RuleDto> GetQueueFakeRulesAlwaysTrue_ChangeStatusDNC()
    {
        var ruleEntry = new RuleEntry()
        {
            Combination = new RuleCombinationData
            {
                Operator = RuleCombinationOperator.True
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
            new(null, "QueueRules", JsonSerializer.Serialize(ruleEntry, JsonSettingsExtensions.Default), 0),
        };
    }

    protected List<RuleDto> GetQueueFakeRulesPreviousStatusBusy_ChangeStatusDNC()
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
                        Name = RulesCondition.PreviousStatus.ToString(),
                        ComparisonOperation = ComparisonOperation.Is,
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
            new(null, "QueueRules", JsonSerializer.Serialize(ruleEntry, SerializerOptions), 0),
        };
    }

    protected List<RuleDto> GetQueueFakeRulesNewLead_SetLeadWeightByXSet20k()
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
                        Name = RulesCondition.NewCampaignLead.ToString(),
                    }
                }
            },
            Actions = new List<RuleActionData>
            {
                new RuleActionData()
                {
                    Name = RulesAction.SetLeadWeightByX.ToString(),
                    ActionOperation = ActionOperation.Set,
                    Fields = new List<RuleValueData>
                    {
                        new RuleValueData()
                        {
                            Type = RuleValueType.Integer,
                            Value = "20000",
                        }
                    }
                }
            }
        };

        return new List<RuleDto>
        {
            new(null, "QueueRules", JsonSerializer.Serialize(ruleEntry, SerializerOptions), 0),
        };
    }

    protected List<RuleDto> GetQueueFakeRulesLeadWeightIsLess2k_SetLeadWeightByXInc2k()
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
                        Name = RulesCondition.CampaignLeadWeightIs.ToString(),
                        ComparisonOperation = ComparisonOperation.LessThan,
                        Fields = new List<RuleValueData>
                        {
                            new RuleValueData()
                            {
                                Type = RuleValueType.Integer,
                                Value = "2000",
                            }
                        }
                    }
                }
            },
            Actions = new List<RuleActionData>
            {
                new RuleActionData()
                {
                    Name = RulesAction.SetLeadWeightByX.ToString(),
                    ActionOperation = ActionOperation.Increase,
                    Fields = new List<RuleValueData>
                    {
                        new RuleValueData()
                        {
                            Type = RuleValueType.Integer,
                            Value = "2000",
                        }
                    }
                }
            }
        };

        return new List<RuleDto>
        {
            new(null, "QueueRules", JsonSerializer.Serialize(ruleEntry, JsonSettingsExtensions.Default), 0),
        };
    }

    protected List<RuleDto> GetQueueFakeRulesLeadWeightMore5k_DeleteLead()
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
                        Name = RulesCondition.CampaignLeadWeightIs.ToString(),
                        ComparisonOperation = ComparisonOperation.MoreThan,
                        Fields = new List<RuleValueData>
                        {
                            new RuleValueData()
                            {
                                Type = RuleValueType.Integer,
                                Value = "5000",
                            }
                        }
                    }
                }
            },
            Actions = new List<RuleActionData>
            {
                new RuleActionData()
                {
                    Name = RulesAction.DeleteLead.ToString()
                }
            }
        };

        return new List<RuleDto>
        {
            new(null, "QueueRules", JsonSerializer.Serialize(ruleEntry, SerializerOptions), 0),
        };
    }

    protected List<RuleDto> GetQueueFakeRulesLeadStatusIsCallAgainPersonal_AssignLead(LeadStatusTypes status)
    {
        var ruleEntry = new RuleEntry()
        {
            Actions = new List<RuleActionData>
            {
                new()
                {
                    Name = RulesAction.AssignAgent.ToString()
                }
            },
            Combination = new RuleCombinationData
            {
                Operator = RuleCombinationOperator.And,
                Groups = new List<RuleGroupData>
                {
                    new RuleGroupData()
                    {
                        Name = RulesCondition.IsFixedAssignedFeedback.ToString(),
                        ComparisonOperation = ComparisonOperation.Equal,
                        Fields = new List<RuleValueData>
                        {
                            new RuleValueData()
                            {
                                Type = RuleValueType.Select,
                                Value = status.ToString(),
                            }
                        }
                    }
                }
            }
        };

        return new List<RuleDto>
        {
            new(null, "QueueRules", JsonSerializer.Serialize(ruleEntry, SerializerOptions), 0),
        };
    }

    protected List<RuleDto> GetQueueFakeRulesCountryUS_ChangeStatusDNC_Multiple()
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
                        Name = RulesCondition.Country.ToString(),
                        ComparisonOperation = ComparisonOperation.Is,
                        Fields = new List<RuleValueData>
                        {
                            new RuleValueData()
                            {
                                Type = RuleValueType.SelectMultiItem,
                                Value = "US,UK"
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
            new(null, "QueueRules", JsonSerializer.Serialize(ruleEntry, SerializerOptions), 0),
        };
    }

    protected List<RuleDto> GetQueueFakeRulesPreviousStatusBusyOrCheckNumber_ChangeStatusDNC()
    {
        var ruleEntry = new RuleEntry()
        {
            Combination = new RuleCombinationData
            {
                Operator = RuleCombinationOperator.Or,
                Groups = new List<RuleGroupData>()
                {
                    new RuleGroupData()
                    {
                        Name = RulesCondition.PreviousStatus.ToString(),
                        ComparisonOperation = ComparisonOperation.Is,
                        Fields = new List<RuleValueData>
                        {
                            new RuleValueData()
                            {
                                Type = RuleValueType.String,
                                Value = LeadStatusTypes.Busy.ToString()
                            }
                        }
                    },
                    new RuleGroupData()
                    {
                        Name = RulesCondition.PreviousStatus.ToString(),
                        ComparisonOperation = ComparisonOperation.Is,
                        Fields = new List<RuleValueData>
                        {
                            new RuleValueData()
                            {
                                Type = RuleValueType.String,
                                Value = LeadStatusTypes.CheckNumber.ToString()
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
            new(null, "QueueRules", JsonSerializer.Serialize(ruleEntry, SerializerOptions), 0),
        };
    }

    protected List<RuleDto> GetQueueFakeRulesPreviousStatusBusyAndScoreMore1000_ChangeStatusDNC()
    {
        var ruleEntry = new RuleEntry()
        {
            Combination = new RuleCombinationData
            {
                Operator = RuleCombinationOperator.And,
                Groups = new List<RuleGroupData>()
                {
                    new RuleGroupData()
                    {
                        Name = RulesCondition.PreviousStatus.ToString(),
                        ComparisonOperation = ComparisonOperation.Is,
                        Fields = new List<RuleValueData>
                        {
                            new RuleValueData()
                            {
                                Type = RuleValueType.String,
                                Value = LeadStatusTypes.Busy.ToString()
                            }
                        }
                    },
                    new RuleGroupData()
                    {
                        Name = RulesCondition.CampaignLeadWeightIs.ToString(),
                        ComparisonOperation = ComparisonOperation.MoreThan,
                        Fields = new List<RuleValueData>
                        {
                            new RuleValueData()
                            {
                                Type = RuleValueType.Integer,
                                Value = "1000"
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
            new(null, "QueueRules", JsonSerializer.Serialize(ruleEntry, SerializerOptions), 0),
        };
    }

    protected List<RuleDto> GetQueueFakeRulesPreviousStatusBusyAndScoreMore5000OrLess1000_ChangeStatusDNC()
    {
        var ruleEntry = new RuleEntry()
        {
            Combination = new RuleCombinationData
            {
                Operator = RuleCombinationOperator.And,
                Groups = new List<RuleGroupData>()
                {
                    new RuleGroupData()
                    {
                        Name = RulesCondition.PreviousStatus.ToString(),
                        ComparisonOperation = ComparisonOperation.Is,
                        Fields = new List<RuleValueData>
                        {
                            new RuleValueData()
                            {
                                Type = RuleValueType.String,
                                Value = LeadStatusTypes.Busy.ToString()
                            }
                        }
                    },
                },
                Combination = new List<RuleCombinationData>
                {
                    new RuleCombinationData()
                    {
                        Operator = RuleCombinationOperator.Or,
                        Groups = new List<RuleGroupData>()
                        {
                            new RuleGroupData()
                            {
                                Name = RulesCondition.CampaignLeadWeightIs.ToString(),
                                ComparisonOperation = ComparisonOperation.MoreThan,
                                Fields = new List<RuleValueData>
                                {
                                    new RuleValueData()
                                    {
                                        Type = RuleValueType.Integer,
                                        Value = "5000"
                                    }
                                }
                            },
                            new RuleGroupData()
                            {
                                Name = RulesCondition.CampaignLeadWeightIs.ToString(),
                                ComparisonOperation = ComparisonOperation.LessThan,
                                Fields = new List<RuleValueData>
                                {
                                    new RuleValueData()
                                    {
                                        Type = RuleValueType.Integer,
                                        Value = "1000"
                                    }
                                }
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
            new(null, "QueueRules", JsonSerializer.Serialize(ruleEntry, SerializerOptions), 0),
        };
    }

    protected List<RuleDto> GetQueueFakeRulesNewStatusWasXTimes_ChangeToMaxCall(int x, int? y,
        ComparisonOperation operation)
    {
        var fields = new List<RuleValueData>
        {
            new RuleValueData()
            {
                Type = RuleValueType.Integer,
                Value = x.ToString()
            }
        };

        if (y.HasValue)
        {
            fields.Add(new RuleValueData()
            {
                Type = RuleValueType.Integer,
                Value = y.ToString()
            });
        }

        var ruleEntry = new RuleEntry()
        {
            Combination = new RuleCombinationData
            {
                Operator = RuleCombinationOperator.None,
                Groups = new List<RuleGroupData>()
                {
                    new RuleGroupData()
                    {
                        Name = y is null
                            ? RulesCondition.NewStatusWasTotalXTimes.ToString()
                            : RulesCondition.NewStatusWasTotalXTimesYPeriod.ToString(),
                        ComparisonOperation = operation,
                        Fields = fields
                    },
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
                            Value = LeadStatusTypes.MaxCall.ToString()
                        }
                    }
                }
            }
        };

        return new List<RuleDto>
        {
            new(null, "QueueRules", JsonSerializer.Serialize(ruleEntry, SerializerOptions), 0),
        };
    }

    protected List<RuleDto> GetQueueFakeRulesCampaignLeadAssignedToAgent(string selectValue)
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
                        Name = RulesCondition.CampaignLeadAssignedToAgent.ToString(),
                        ComparisonOperation = ComparisonOperation.Is,
                        Fields = new List<RuleValueData>
                        {
                            new RuleValueData()
                            {
                                Type = RuleValueType.Select,
                                Value = selectValue
                            }
                        }
                    },
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
                            Value = LeadStatusTypes.MaxCall.ToString()
                        }
                    }
                }
            }
        };

        return new List<RuleDto>
        {
            new(null, "QueueRules", JsonSerializer.Serialize(ruleEntry, SerializerOptions), 0),
        };
    }

    protected List<RuleDto> GetQueueFakeRulesCampaignLeadTotalCalls(int x, int? y, ComparisonOperation operation)
    {
        using (var context = new DialDbContext(DbContextOptions))
        {
            context.CallDetailRecords.RemoveRange(context.CallDetailRecords.ToArray());

            for (var i = 1; i <= 3; i++)
            {
                context.CallDetailRecords.Add(new CallDetailRecord
                {
                    Id = 100 + i,
                    LeadId = 1,
                    Brand = "NameTest1",
                    OriginatedAt = DateTimeOffset.UtcNow.AddDays(-(5 + i)).AddMinutes(-(15 + i * 10)),
                    CallHangupAt = DateTimeOffset.UtcNow.AddDays(-(5 + i)).AddMinutes(-(170 + i * 10)),
                    LeadAnswerAt = DateTimeOffset.UtcNow.AddDays(-(5 + i)).AddMinutes(-(200 + i * 10)),
                    UserAnswerAt = DateTimeOffset.UtcNow.AddDays(-(5 + i)).AddMinutes(-(199 + i * 10)),
                    CallHangupStatus = CallFinishReasons.CallFinishedByAgent,
                    CallType = CallType.Predictive,
                    CallerId = 1.ToString(),
                    ClientId = 1,
                    SessionId = "0e097be3-650b-4ee4-9d36-d209729b9066",
                    LeadName = "WEN test    ",
                    LeadPhone = "380982753376",
                    LeadCountry = "AR",
                    LeadQueueId = 1,
                    LeadQueueName = "Online Group",
                    LeadStatusAfter = null,
                    LeadStatusBefore = LeadStatusTypes.NewLead,
                    LastUserId = i + 10,
                    LastUserName = "Brad Pitt",
                    RecordUserFiles = "0e097be3-650b-4ee4-9d36-d209729b9066/20230220T154210.938_380982753376_agent.ogg",
                    RecordLeadFile = "0e097be3-650b-4ee4-9d36-d209729b9066/20230220T154210.007_380982753376_lead.ogg",
                    RecordManagerFiles = null,
                    RecordMixedFile = "0e097be3-650b-4ee4-9d36-d209729b9066/20230220T154210.007_380982753376.ogg",
                    IsReplacedUser = false,
                    MetaData = null,
                });
            }

            context.SaveChanges();
        }

        var fields = new List<RuleValueData>
        {
            new RuleValueData()
            {
                Type = RuleValueType.Integer,
                Value = x.ToString()
            }
        };

        if (y.HasValue)
        {
            fields.Add(new RuleValueData()
            {
                Type = RuleValueType.Integer,
                Value = y.ToString()
            });
        }

        var ruleEntry = new RuleEntry()
        {
            Combination = new RuleCombinationData
            {
                Operator = RuleCombinationOperator.None,
                Groups = new List<RuleGroupData>()
                {
                    new RuleGroupData()
                    {
                        Name = y is null
                            ? RulesCondition.LeadHadTotalOfXCalls.ToString()
                            : RulesCondition.LeadHadTotalOfXCallsYPeriod.ToString(),
                        ComparisonOperation = operation,
                        Fields = fields
                    },
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
                            Value = LeadStatusTypes.MaxCall.ToString()
                        }
                    }
                }
            }
        };

        return new List<RuleDto>
        {
            new(null, "QueueRules", JsonSerializer.Serialize(ruleEntry, SerializerOptions), 0),
        };
    }

    protected List<CallDetailRecord> GenerateCdRs(IEnumerable<TrackedLead> leads)
    {
        var cdrList = new List<CallDetailRecord>();
        using var context = new DialDbContext(DbContextOptions);
        context.CallDetailRecords.RemoveRange(context.CallDetailRecords.ToArray());
        var r = new Random();
        foreach (var lead in leads)
        {
            for (var i = 1; i <= 4; i++)
            {
                cdrList.Add(new CallDetailRecord
                {
                    Id = long.Parse($"{lead.LeadId}{i}"),
                    LeadId = lead.LeadId,
                    Brand = "NameTest1",
                    OriginatedAt = DateTimeOffset.UtcNow.AddDays(-(5 + i)).AddMinutes(-60),
                    LeadAnswerAt = DateTimeOffset.UtcNow.AddDays(-(5 + i)).AddMinutes(-59),
                    UserAnswerAt = DateTimeOffset.UtcNow.AddDays(-(5 + i)).AddMinutes(-58),
                    CallHangupAt = DateTimeOffset.UtcNow.AddDays(-(5 + i)).AddMinutes(-28),
                    CallHangupStatus = CallFinishReasons.CallFinishedByAgent,
                    CallType = CallType.Predictive,
                    CallerId = 1.ToString(),
                    ClientId = 1,
                    SessionId = "0e097be3-650b-4ee4-9d36-d209729b9066",
                    LeadName = "WEN TSET    ",
                    LeadPhone = "380982753376",
                    LeadCountry = "AR",
                    LeadQueueId = 1,
                    LeadQueueName = "Online Group",
                    LeadStatusAfter = null,
                    LeadStatusBefore = LeadStatusTypes.NewLead,
                    LastUserId = i + 10,
                    LastUserName = "Brad Pitt",
                    RecordUserFiles = "0e097be3-650b-4ee4-9d36-d209729b9066/20230220T154210.938_380982753376_agent.ogg",
                    RecordLeadFile = "0e097be3-650b-4ee4-9d36-d209729b9066/20230220T154210.007_380982753376_lead.ogg",
                    RecordManagerFiles = null,
                    RecordMixedFile = "0e097be3-650b-4ee4-9d36-d209729b9066/20230220T154210.007_380982753376.ogg",
                    IsReplacedUser = false,
                    MetaData = null,
                    CallDuration = r.Next(1, 3000)
                });
            }
        }

        context.CallDetailRecords.AddRange(cdrList);
        context.SaveChanges();

        return cdrList;
    }

    protected List<RuleDto> GetQueueFakeRulesCampaignLeadCallsSeconds(int x, ComparisonOperation operation)
    {
        var fields = new List<RuleValueData>
        {
            new RuleValueData()
            {
                Type = RuleValueType.Integer,
                Value = x.ToString()
            }
        };

        var ruleEntry = new RuleEntry()
        {
            Combination = new RuleCombinationData
            {
                Operator = RuleCombinationOperator.None,
                Groups = new List<RuleGroupData>()
                {
                    new RuleGroupData()
                    {
                        Name = RulesCondition.CallDuration.ToString(),
                        ComparisonOperation = operation,
                        Fields = fields
                    },
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
                            Value = LeadStatusTypes.MaxCall.ToString()
                        }
                    }
                }
            }
        };

        return new List<RuleDto>
        {
            new(null, "QueueRules", JsonSerializer.Serialize(ruleEntry, SerializerOptions), 0),
        };
    }

    protected List<RuleDto> GetQueueFakeRulesCampaignLeadPrevStatus(LeadStatusTypes leadStatus,
        ComparisonOperation operation)
    {
        var fields = new List<RuleValueData>
        {
            new RuleValueData()
            {
                Type = RuleValueType.Select,
                Value = leadStatus.ToString()
            }
        };

        var ruleEntry = new RuleEntry()
        {
            Combination = new RuleCombinationData
            {
                Operator = RuleCombinationOperator.None,
                Groups = new List<RuleGroupData>()
                {
                    new RuleGroupData()
                    {
                        Name = RulesCondition.PreviousStatus.ToString(),
                        ComparisonOperation = operation,
                        Fields = fields
                    },
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
                            Value = LeadStatusTypes.MaxCall.ToString()
                        }
                    }
                }
            }
        };

        return new List<RuleDto>
        {
            new(null, "QueueRules", JsonSerializer.Serialize(ruleEntry, SerializerOptions), 0),
        };
    }

    protected List<RuleDto> GetQueueFakeRulesWithSameRulesActions()
    {
        var ruleEntry1 = new RuleEntry
        {
            Combination = new RuleCombinationData
            {
                Operator = RuleCombinationOperator.True
            },
            Actions = new List<RuleActionData>
            {
                new RuleActionData
                {
                    Name = RulesAction.ChangeStatus.ToString(),
                    ActionOperation = ActionOperation.To,
                    Fields = new List<RuleValueData>
                    {
                        new RuleValueData
                        {
                            Type = RuleValueType.String,
                            Value = LeadStatusTypes.DNC.ToString()
                        }
                    }
                }
            }
        };

        var ruleEntry2 = new RuleEntry
        {
            Combination = new RuleCombinationData
            {
                Operator = RuleCombinationOperator.None,
                Groups = new List<RuleGroupData>()
                {
                    new RuleGroupData()
                    {
                        Name = RulesCondition.CampaignLeadWeightIs.ToString(),
                        ComparisonOperation = ComparisonOperation.LessThan,
                        Fields = new List<RuleValueData>
                        {
                            new RuleValueData()
                            {
                                Type = RuleValueType.Integer,
                                Value = "2000",
                            }
                        }
                    }
                }
            },
            Actions = new List<RuleActionData>
            {
                new RuleActionData
                {
                    Name = RulesAction.ChangeStatus.ToString(),
                    ActionOperation = ActionOperation.To,
                    Fields = new List<RuleValueData>
                    {
                        new RuleValueData
                        {
                            Type = RuleValueType.String,
                            Value = LeadStatusTypes.CallAgainGeneral.ToString()
                        }
                    }
                }
            }
        };

        return new List<RuleDto>
        {
            new(null, "QueueRules", JsonSerializer.Serialize(ruleEntry1, SerializerOptions), 0),
            new(null, "QueueRules", JsonSerializer.Serialize(ruleEntry2, SerializerOptions), 0),
        };
    }

    protected List<RuleDto> GetQueueFakeRulesWithSameRulesConditions()
    {
        var ruleEntry1 = new RuleEntry
        {
            Combination = new RuleCombinationData
            {
                Operator = RuleCombinationOperator.None,
                Groups = new List<RuleGroupData>
                {
                    new RuleGroupData
                    {
                        Name = RulesCondition.Country.ToString(),
                        ComparisonOperation = ComparisonOperation.Is,
                        Fields = new List<RuleValueData>
                        {
                            new RuleValueData
                            {
                                Type = RuleValueType.SelectMultiItem,
                                Value = "UA",
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

        var ruleEntry2 = new RuleEntry
        {
            Combination = new RuleCombinationData
            {
                Operator = RuleCombinationOperator.None,
                Groups = new List<RuleGroupData>
                {
                    new RuleGroupData
                    {
                        Name = RulesCondition.Country.ToString(),
                        ComparisonOperation = ComparisonOperation.Is,
                        Fields = new List<RuleValueData>
                        {
                            new RuleValueData
                            {
                                Type = RuleValueType.SelectMultiItem,
                                Value = "SE",
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
            new(null, "QueueRules", JsonSerializer.Serialize(ruleEntry1, SerializerOptions), 0),
            new(null, "QueueRules", JsonSerializer.Serialize(ruleEntry2, SerializerOptions), 0),
        };
    }

    protected List<RuleDto> GetFixedAssignedFeedbackFakeRules()
    {
        var ruleEntry1 = new RuleEntry
        {
            Combination = new RuleCombinationData
            {
                Operator = RuleCombinationOperator.None,
                Groups = new List<RuleGroupData>
                {
                    new RuleGroupData
                    {
                        Name = RulesCondition.IsFixedAssignedFeedback.ToString(),
                        Fields = new List<RuleValueData>
                        {
                            new RuleValueData
                            {
                                Type = RuleValueType.Select,
                                Value = LeadStatusTypes.CallAgainPersonal.ToString(),
                            }
                        }
                    }
                }
            },
            Actions = new List<RuleActionData>
            {
                new RuleActionData()
                {
                    Name = RulesAction.ChangeLeadFieldsValue.ToString(),
                    ActionOperation = ActionOperation.Set,
                    Fields = new List<RuleValueData>
                    {
                        // new()
                        // {
                        //     Type = RuleValueType.Select,
                        //     Value = nameof(TrackedLead.IsFixedAssigned)
                        // },
                        // new()
                        // {
                        //     Type = RuleValueType.String,
                        //     Value = true.ToString(),
                        // }
                    }
                }
            }
        };

        return new List<RuleDto>
        {
            new(null, "FixedAssignedFeedbackRules", JsonSerializer.Serialize(ruleEntry1, SerializerOptions), 0),
        };
    }

    protected List<RuleDto> GetFutureCallMultipleFakeRules()
    {
        var ruleEntry1 = new RuleEntry
        {
            Combination = new RuleCombinationData
            {
                Operator = RuleCombinationOperator.None,
                Groups = new List<RuleGroupData>
                {
                    new()
                    {
                        Name = RulesCondition.IsFutureCall.ToString(),
                    },
                }
            },
            Actions = new List<RuleActionData>
            {
                new()
                {
                    Name = RulesAction.MoveLeadToQueue.ToString(),
                    ActionOperation = ActionOperation.To,
                    Fields = new List<RuleValueData>
                    {
                        new RuleValueData()
                        {
                            Type = RuleValueType.Select,
                            Value = 11.ToString(),
                        },
                    }
                }
            }
        };

        return new List<RuleDto>
        {
            new(null, "FutureRules", JsonSerializer.Serialize(ruleEntry1, SerializerOptions), 0),
        };
    }

    protected List<RuleDto> GetCurrentStatusFakeRules()
    {
        var ruleEntry1 = new RuleEntry
        {
            Combination = new RuleCombinationData
            {
                Operator = RuleCombinationOperator.None,
                Groups = new List<RuleGroupData>
                {
                    new()
                    {
                        Name = RulesCondition.CurrentStatus.ToString(),
                        ComparisonOperation = ComparisonOperation.Is,
                        Fields = new List<RuleValueData>
                        {
                            new RuleValueData
                            {
                                Type = RuleValueType.SelectMultiItem,
                                Value = string.Join(",",
                                    new[] { LeadStatusTypes.CallAgainPersonal, LeadStatusTypes.NewLead }),
                            }
                        }
                    },
                }
            },
            Actions = new List<RuleActionData>
            {
                new()
                {
                    Name = RulesAction.MoveLeadToQueue.ToString(),
                    ActionOperation = ActionOperation.To,
                    Fields = new List<RuleValueData>
                    {
                        new RuleValueData()
                        {
                            Type = RuleValueType.Select,
                            Value = 11.ToString(),
                        },
                    }
                }
            }
        };

        return new List<RuleDto>
        {
            new(null, "FutureRules", JsonSerializer.Serialize(ruleEntry1, JsonSettingsExtensions.Default), 0),
        };
    }


    protected static Task<string?> GetFakeRuleSettings()
    {
        return Task.FromResult((string?)
            @"
            {
            ""leadFields"": [
                {
                ""name"": ""group_ids"",
                ""type"": ""set"",
                ""displayName"": ""Groups IDs"",
                ""setElementType"": ""integer""
                },
                {
                ""name"": ""campaign_id"",
                ""type"": ""identifier"",
                ""displayName"": ""Campaign ID""
                },
                {
                ""name"": ""sub_campaign"",
                ""type"": ""identifier"",
                ""displayName"": ""Subcampaign ID""
                },
                {
                ""name"": ""department_id"",
                ""type"": ""identifier"",
                ""displayName"": ""Department ID""
                },
                {
                ""name"": ""score"",
                ""type"": ""integer"",
                ""displayName"": ""Score""
                },
                {
                ""name"": ""campaign_name"",
                ""type"": ""string"",
                ""displayName"": ""Campaign Name""
                },
                {
                ""name"": ""campaign_type"",
                ""type"": ""set"",
                ""displayName"": ""Campaign Type"",
                ""setElementType"": ""string""
                }
            ]
            }"
        );
    }


    protected List<RuleDto> GetQueueFakeRulesLeadFieldCampaignId()
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
                        Name = RulesCondition.LeadField.ToString() + "_campaign_id",
                        ComparisonOperation = ComparisonOperation.Equal,
                        Fields = new List<RuleValueData>
                        {
                            new RuleValueData
                            {
                                Type = RuleValueType.Integer,
                                Value = "1",
                            }
                        }
                    }
                }
            },
            Actions = new List<RuleActionData>
            {
                new()
                {
                    Name = RulesAction.MoveLeadToQueue.ToString(),
                    ActionOperation = ActionOperation.To,
                    Fields = new List<RuleValueData>
                    {
                        new RuleValueData()
                        {
                            Type = RuleValueType.Select,
                            Value = 11.ToString(),
                        },
                    }
                }
            }
        };

        return new List<RuleDto>
        {
            new(null, "QueueRules", JsonSerializer.Serialize(ruleEntry, SerializerOptions), 0),
        };
    }

    protected List<RuleDto> GetQueueFakeRulesLeadFieldScore()
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
                        Name = RulesCondition.LeadField.ToString() + "_score",
                        ComparisonOperation = ComparisonOperation.LessThanEqual,
                        Fields = new List<RuleValueData>
                        {
                            new RuleValueData
                            {
                                Type = RuleValueType.Integer,
                                Value = "150",
                            }
                        }
                    }
                }
            },
            Actions = new List<RuleActionData>
            {
                new()
                {
                    Name = RulesAction.MoveLeadToQueue.ToString(),
                    ActionOperation = ActionOperation.To,
                    Fields = new List<RuleValueData>
                    {
                        new RuleValueData()
                        {
                            Type = RuleValueType.Select,
                            Value = 11.ToString(),
                        },
                    }
                }
            }
        };

        return new List<RuleDto>
        {
            new(null, "QueueRules", JsonSerializer.Serialize(ruleEntry, SerializerOptions), 0),
        };
    }

    protected List<RuleDto> GetQueueFakeRulesLeadFieldCampaignName()
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
                        Name = RulesCondition.LeadField.ToString() + "_campaign_name",
                        ComparisonOperation = ComparisonOperation.Contains,
                        Fields = new List<RuleValueData>
                        {
                            new RuleValueData
                            {
                                Type = RuleValueType.String,
                                Value = "!_test_!",
                            }
                        }
                    }
                }
            },
            Actions = new List<RuleActionData>
            {
                new()
                {
                    Name = RulesAction.MoveLeadToQueue.ToString(),
                    ActionOperation = ActionOperation.To,
                    Fields = new List<RuleValueData>
                    {
                        new RuleValueData()
                        {
                            Type = RuleValueType.Select,
                            Value = 11.ToString(),
                        },
                    }
                }
            }
        };

        return new List<RuleDto>
        {
            new(null, "QueueRules", JsonSerializer.Serialize(ruleEntry, SerializerOptions), 0),
        };
    }


    protected List<RuleDto> GetQueueFakeRulesLeadFieldGroupIdsContains()
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
                        Name = RulesCondition.LeadField.ToString() + "_group_ids",
                        ComparisonOperation = ComparisonOperation.Contains,
                        Fields = new List<RuleValueData>
                        {
                            new RuleValueData
                            {
                                Type = RuleValueType.IntegerSet,
                                Value = "2,4,6",
                            }
                        }
                    }
                }
            },
            Actions = new List<RuleActionData>
            {
                new()
                {
                    Name = RulesAction.MoveLeadToQueue.ToString(),
                    ActionOperation = ActionOperation.To,
                    Fields = new List<RuleValueData>
                    {
                        new RuleValueData()
                        {
                            Type = RuleValueType.Select,
                            Value = 11.ToString(),
                        },
                    }
                }
            }
        };

        return new List<RuleDto>
        {
            new(null, "QueueRules", JsonSerializer.Serialize(ruleEntry, SerializerOptions), 0),
        };
    }

    protected List<RuleDto> GetQueueFakeRulesLeadFieldGroupIdsEqual()
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
                        Name = RulesCondition.LeadField.ToString() + "_group_ids",
                        ComparisonOperation = ComparisonOperation.Equal,
                        Fields = new List<RuleValueData>
                        {
                            new RuleValueData
                            {
                                Type = RuleValueType.IntegerSet,
                                Value = "1,2,3,4",
                            }
                        }
                    }
                }
            },
            Actions = new List<RuleActionData>
            {
                new()
                {
                    Name = RulesAction.MoveLeadToQueue.ToString(),
                    ActionOperation = ActionOperation.To,
                    Fields = new List<RuleValueData>
                    {
                        new RuleValueData()
                        {
                            Type = RuleValueType.Select,
                            Value = 11.ToString(),
                        },
                    }
                }
            }
        };

        return new List<RuleDto>
        {
            new(null, "QueueRules", JsonSerializer.Serialize(ruleEntry, SerializerOptions), 0),
        };
    }


    protected List<RuleDto> GetQueueFakeRulesLeadFieldCampaignTypeContains()
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
                        Name = RulesCondition.LeadField.ToString() + "_campaign_type",
                        ComparisonOperation = ComparisonOperation.Contains,
                        Fields = new List<RuleValueData>
                        {
                            new RuleValueData
                            {
                                Type = RuleValueType.StringSet,
                                Value = "test1,test3,test8",
                            }
                        }
                    }
                }
            },
            Actions = new List<RuleActionData>
            {
                new()
                {
                    Name = RulesAction.MoveLeadToQueue.ToString(),
                    ActionOperation = ActionOperation.To,
                    Fields = new List<RuleValueData>
                    {
                        new RuleValueData()
                        {
                            Type = RuleValueType.Select,
                            Value = 11.ToString(),
                        },
                    }
                }
            }
        };

        return new List<RuleDto>
        {
            new(null, "QueueRules", JsonSerializer.Serialize(ruleEntry, SerializerOptions), 0),
        };
    }

    // protected static List<RuleDto> GetQueueFakeRules()
    // {
    //     var rules = new Rule[]
    //     {
    //         new()
    //         {
    //             RuleName = "Status_NewLeas_And_Score_More999",
    //             Operator = "And",
    //             Rules = new[]
    //             {
    //                 new Rule
    //                 {
    //                     RuleName = "Status_NewLeas",
    //                     Expression = $"{LeadInputName}.Status == \"NewLead\"",
    //                 },
    //                 new Rule
    //                 {
    //                     RuleName = "Score_More999",
    //                     Expression = $"{LeadInputName}.Score > 999",
    //                 }
    //             },
    //             Actions = new RuleActions
    //             {
    //                 OnSuccess = new ActionInfo
    //                 {
    //                     Name = "OutputExpression",
    //                     Context = new Dictionary<string, object> { { "Expression", "1" } },
    //                 }
    //             },
    //         },
    //         new()
    //         {
    //             RuleName = "Status_MoveToOldBrands",
    //             Expression = $"{LeadInputName}.Status == \"MoveToOldBrands\"",
    //             Actions = new RuleActions
    //             {
    //                 OnSuccess = new ActionInfo
    //                 {
    //                     Name = "OutputExpression",
    //                     Context = new Dictionary<string, object> { { "Expression", "15" } },
    //                 }
    //             },
    //         },
    //         new()
    //         {
    //             RuleName = "DefaultQueue",
    //             Expression = "true",
    //             Actions = new RuleActions
    //             {
    //                 OnSuccess = new ActionInfo
    //                 {
    //                     Name = "OutputExpression",
    //                     Context = new Dictionary<string, object> { { "Expression", "99" } },
    //                 }
    //             },
    //         },
    //     };

    //     return new List<RuleDto>
    //     {
    //         new(null, "QueueRules", JsonSerializer.Serialize(rules), 0),
    //     };
    // }

    // protected static List<RuleDto> GetScoreFakeRules()
    // {
    //     var rules = new Rule[]
    //     {
    //         new()
    //         {
    //             RuleName = "IncreaseScore",
    //             Operator = "And",
    //             Actions = new RuleActions
    //             {
    //                 OnSuccess = new ActionInfo
    //                 {
    //                     Name = "OutputExpression",
    //                     Context = new Dictionary<string, object> { { "Expression", "1000" } },
    //                 }
    //             },
    //             Rules = new []
    //             {
    //                 new Rule
    //                 {
    //                     RuleName = "ForNewLead",
    //                     Expression = $"{LeadInputName}.Status == \"NewLead\"",
    //                 },
    //                 new Rule
    //                 {
    //                     RuleName = "ForRegisteredMax30MinutesAgo",
    //                     Expression = $"{LeadInputName}.MinutesSinceRegistration <= 30 AND {LeadInputName}.MinutesSinceRegistration > 0",
    //                 }
    //             }
    //         }
    //     };

    //     return new List<RuleDto>
    //     {
    //         new(null, "ScoreRules", JsonSerializer.Serialize(rules), 0),
    //     };
    // }

    #endregion
}