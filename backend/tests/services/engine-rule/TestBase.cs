using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Castle.Components.DictionaryAdapter;
using Plat4Me.DialRuleEngine.Application.Enums;
using Plat4Me.DialRuleEngine.Application.Extensions;
using Plat4Me.DialRuleEngine.Application.Models;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Contracts;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Enums;

namespace Plat4Me.DialRuleEngine.Tests;

public class TestBase
{
    private long _leadId = 1;

    #region Conditions

    protected RuleGroupData NewStatusWasTotalXTimesYPeriod_Condition
    (
        ComparisonOperation operation,
        int calls,
        int period
    )
    {
        return new()
        {
            Name = RulesCondition.NewStatusWasTotalXTimesYPeriod.ToString(),
            ComparisonOperation = operation,
            Fields = new EditableList<RuleValueData>
            {
                new()
                {
                    Type = RuleValueType.Integer,
                    Value = calls.ToString()
                },
                new()
                {
                    Type = RuleValueType.Integer,
                    Value = period.ToString()
                },
            },
        };
    }

    protected RuleGroupData CurrentStatus_Condition(
        ComparisonOperation operation,
        params LeadStatusTypes[] leadStatuses)
    {
        return new()
        {
            Name = RulesCondition.CurrentStatus.ToString(),
            ComparisonOperation = operation,
            Fields = new List<RuleValueData>
            {
                new()
                {
                    Type = RuleValueType.SelectMultiItem,
                    Value = string.Join(",", leadStatuses),
                }
            }
        };
    }

    protected RuleGroupData LeadHadTotalOfXCallsYPeriod_Condition(
        ComparisonOperation operation,
        int calls,
        int period)
    {
        return new()
        {
            Name = RulesCondition.LeadHadTotalOfXCallsYPeriod.ToString(),
            ComparisonOperation = operation,
            Fields = new EditableList<RuleValueData>
            {
                new()
                {
                    Type = RuleValueType.Integer,
                    Value = calls.ToString()
                },
                new()
                {
                    Type = RuleValueType.Integer,
                    Value = period.ToString()
                },
            },
        };
    }

    protected RuleGroupData NewStatusWasXConsecutiveTimesYPeriod_Condition
    (
        ComparisonOperation operation,
        int times,
        int lastTime
    )
    {
        return new()
        {
            Name = RulesCondition.NewStatusWasXConsecutiveTimesYPeriod.ToString(),
            ComparisonOperation = operation,
            Fields = new EditableList<RuleValueData>
            {
                new()
                {
                    Type = RuleValueType.Integer,
                    Value = times.ToString()
                },
                new()
                {
                    Type = RuleValueType.Integer,
                    Value = lastTime.ToString()
                },
            }
        };
    }

    protected RuleGroupData IsFutureCall_Condition()
    {
        return new()
        {
            Name = RulesCondition.IsFutureCall.ToString(),
        };
    }

    protected RuleGroupData IsFixedAssignedFeedback_Condition(LeadStatusTypes status)
    {
        return new()
        {
            Name = RulesCondition.IsFixedAssignedFeedback.ToString(),
            Fields = new List<RuleValueData>
            {
                new()
                {
                    Type = RuleValueType.Select,
                    Value = status.ToString(),
                }
            }
        };
    }

    protected RuleGroupData IsTestLead_Condition(ComparisonOperation operation)
    {
        return new()
        {
            Name = RulesCondition.IsTestLead.ToString(),
            ComparisonOperation = operation
        };
    }

    protected RuleGroupData IsImported_Condition()
    {
        return new()
        {
            Name = RulesCondition.IsImported.ToString(),
        };
    }

    protected RuleGroupData LeadIsInTheSystem_Condition(ComparisonOperation operation, int count, TimeUnits unit)
    {
        return new()
        {
            Name = RulesCondition.LeadIsInSystem.ToString(),
            ComparisonOperation = operation,
            Fields = new List<RuleValueData>
            {
                new()
                {
                    Type = RuleValueType.Integer,
                    Value = count.ToString(),
                },
                new()
                {
                    Type = RuleValueType.Select,
                    Value = unit.ToString(),
                },
            }
        };
    }

    protected RuleGroupData Country_Condition(ComparisonOperation operation, params string[] countries)
    {
        return new()
        {
            Name = RulesCondition.Country.ToString(),
            ComparisonOperation = operation,
            Fields = new List<RuleValueData>
            {
                new()
                {
                    Type = RuleValueType.SelectMultiItem,
                    Value = string.Join(",", countries)
                }
            }
        };
    }

    protected RuleGroupData CallDuration_Condition(ComparisonOperation operation, int duration)
    {
        return new()
        {
            Name = RulesCondition.CallDuration.ToString(),
            ComparisonOperation = operation,
            Fields = new List<RuleValueData>
            {
                new()
                {
                    Type = RuleValueType.Integer,
                    Value = duration.ToString()
                }
            }
        };
    }

    protected RuleGroupData NewCampaignLead_Condition()
    {
        return new()
        {
            Name = RulesCondition.NewCampaignLead.ToString(),
        };
    }

    protected RuleGroupData LeadHadTotalOfXCalls_Condition(ComparisonOperation operation, int count)
    {
        return new()
        {
            Name = RulesCondition.LeadHadTotalOfXCalls.ToString(),
            ComparisonOperation = operation,
            Fields = new List<RuleValueData>
            {
                new()
                {
                    Type = RuleValueType.Integer,
                    Value = count.ToString()
                }
            }
        };
    }

    protected RuleGroupData CampaignLeadAssignedToAgent_Condition(bool isEquals)
    {
        return new()
        {
            Name = RulesCondition.CampaignLeadAssignedToAgent.ToString(),
            ComparisonOperation = ComparisonOperation.Is,
            Fields = new List<RuleValueData>
            {
                new()
                {
                    Type = RuleValueType.Select,
                    Value = isEquals.ToString()
                }
            }
        };
    }

    protected RuleGroupData NewStatusWasXConsecutiveTimes_Condition(ComparisonOperation operation, int countTimes)
    {
        return new()
        {
            Name = RulesCondition.NewStatusWasXConsecutiveTimes.ToString(),
            ComparisonOperation = operation,
            Fields = new List<RuleValueData>
            {
                new()
                {
                    Type = RuleValueType.Integer,
                    Value = countTimes.ToString()
                }
            }
        };
    }

    protected RuleGroupData NewStatusWasTotalXTimes_Condition(ComparisonOperation operation, int countTimes)
    {
        return new()
        {
            Name = RulesCondition.NewStatusWasTotalXTimes.ToString(),
            ComparisonOperation = operation,
            Fields = new List<RuleValueData>
            {
                new()
                {
                    Type = RuleValueType.Integer,
                    Value = countTimes.ToString()
                }
            }
        };
    }

    protected RuleGroupData CampaignLeadWeightIs_Condition(ComparisonOperation operation, int countTimes)
    {
        return new()
        {
            Name = RulesCondition.CampaignLeadWeightIs.ToString(),
            ComparisonOperation = operation,
            Fields = new List<RuleValueData>
            {
                new()
                {
                    Type = RuleValueType.Integer,
                    Value = countTimes.ToString()
                }
            }
        };
    }

    protected RuleGroupData NewStatus_Condition(ComparisonOperation operation, LeadStatusTypes status)
    {
        return new()
        {
            Name = RulesCondition.NewStatus.ToString(),
            ComparisonOperation = operation,
            Fields = new List<RuleValueData>
            {
                new()
                {
                    Type = RuleValueType.Select,
                    Value = status.ToString()
                }
            }
        };
    }

    protected RuleGroupData PreviousStatus_Condition(ComparisonOperation operation, LeadStatusTypes status)
    {
        return new()
        {
            Name = RulesCondition.PreviousStatus.ToString(),
            ComparisonOperation = operation,
            Fields = new List<RuleValueData>
            {
                new()
                {
                    Type = RuleValueType.Select,
                    Value = status.ToString()
                }
            }
        };
    }

    #endregion

    #region Actions

    protected RuleActionData AssignAgent_Action()
    {
        return new()
        {
            Name = RulesAction.AssignAgent.ToString(),
        };
    }

    protected RuleActionData MoveLeadToQueue_Action(long queueId = 1)
    {
        return new()
        {
            Name = RulesAction.MoveLeadToQueue.ToString(),
            ActionOperation = ActionOperation.To,
            Fields = new List<RuleValueData>
            {
                new()
                {
                    Type = RuleValueType.Select,
                    Value = queueId.ToString(),
                },
            }
        };
    }

    protected RuleActionData SetLeadWeightByX_Action(ActionOperation operation, long weight)
    {
        return new()
        {
            Name = RulesAction.SetLeadWeightByX.ToString(),
            ActionOperation = operation,
            Fields = new List<RuleValueData>
            {
                new()
                {
                    Type = RuleValueType.Integer,
                    Value = weight.ToString(),
                },
            }
        };
    }

    protected RuleActionData ChangeLeadFieldsValue_Action(string fieldName, string value)
    {
        return new()
        {
            Name = RulesAction.ChangeLeadFieldsValue.ToString(),
            ActionOperation = ActionOperation.Set,
            Fields = new List<RuleValueData>
            {
                new()
                {
                    Type = RuleValueType.Select,
                    Value = fieldName
                },
                new()
                {
                    Type = RuleValueType.String,
                    Value = value,
                }
            }
        };
    }

    protected RuleActionData DeleteLead_Action()
    {
        return new()
        {
            Name = RulesAction.DeleteLead.ToString(),
        };
    }

    protected RuleActionData ChangeStatus_Action(LeadStatusTypes status)
    {
        return new()
        {
            Name = RulesAction.ChangeStatus.ToString(),
            ActionOperation = ActionOperation.To,
            Fields = new List<RuleValueData>
            {
                new()
                {
                    Type = RuleValueType.String,
                    Value = status.ToString()
                }
            }
        };
    }

    protected RuleActionData FreezeLeadForX_Action(int count, TimeUnits units)
    {
        return new()
        {
            Name = RulesAction.FreezeLeadForX.ToString(),
            Fields = new List<RuleValueData>
            {
                new()
                {
                    Type = RuleValueType.Integer,
                    Value = count.ToString()
                },
                new()
                {
                    Type = RuleValueType.Select,
                    Value = units.ToString()
                },
            }
        };
    }

    #endregion

    #region Utils

    protected TrackedLead GetLead_Default(
        long? queueId = null,
        LeadStatusTypes status = LeadStatusTypes.NewLead,
        LeadSystemStatusTypes? systemStatus = null,
        DateTimeOffset? registrationTime = null,
        DateTimeOffset? remindOn = null,
        long? lastCallAgentId = null,
        string? countryCode = null,
        DateTimeOffset? deletedOn = null,
        string? timezone = null,
        bool? isTest = null,
        long? assignedAgentId = null,
        DateTimeOffset? freezeTo = null,
        IEnumerable<KeyValuePair<DateTimeOffset, string>>? statusHistory = null,
        long? score = null)
    {
        var index = _leadId++;
        return new TrackedLead(
            leadId: index,
            firstName: "test-first-name-" + index,
            lastName: "test-last-name-" + index,
            leadPhone: "test-phone-" + index,
            status,
            systemStatus,
            registrationTime ?? DateTimeOffset.Now.AddMinutes(-1),
            remindOn,
            lastCallAgentId,
            countryCode,
            deletedOn,
            timezone,
            isTest ?? false,
            assignedAgentId,
            freezeTo,
            statusHistory, null)
        {
            LeadQueueId = queueId,
            Score = score.GetValueOrDefault()
        };
    }

    public static IReadOnlyCollection<RuleDto> Combine(
        IEnumerable<RuleGroupData> conditions,
        RuleCombinationOperator combinationOperator,
        params RuleActionData[] actions)
    {
        var rule = new RuleEntry
        {
            Combination = new RuleCombinationData
            {
                Operator = combinationOperator,
                Groups = conditions.ToList(),
            },
            Actions = actions.ToList()
        };
        return new[]
        {
            new RuleDto(
                QueueId: null,
                Name: "test-queue-rules-" + 1,
                Rules: JsonSerializer.Serialize(rule, JsonSettingsExtensions.Default),
                Ordinal: 0)
        };
    }

    public static IReadOnlyCollection<RuleDto> Combine(RuleGroupData condition,
        RuleCombinationOperator combinationOperator,
        params RuleActionData[] actions)
    {
        return Combine(new[] { condition }, combinationOperator, actions);
    }
    
    protected List<KeyValuePair<DateTimeOffset, string>> PrepareLeadStatusHistory(LeadStatusTypes status,
        int countChanges)
    {
        var statusChangeHistory1 = new List<ValueChanges<object?>> { new(nameof(TrackedLead.Status), status, status) };
        var statusChangeHistoryDto1 = new LeadHistoryChangesDto<object?> { Properties = statusChangeHistory1 };

        var statusHistoryArray = new List<KeyValuePair<DateTimeOffset, string>>();
        for (var i = 0; i < countChanges; i++)
        {
            statusHistoryArray.Add(new KeyValuePair<DateTimeOffset, string>(DateTimeOffset.UtcNow.AddDays(i),
                JsonSerializer.Serialize(statusChangeHistoryDto1, JsonSettingsExtensions.Default)));
        }

        return statusHistoryArray;
    }

    #endregion
}

public static class TestBaseExtension
{
    public static IReadOnlyCollection<RuleDto> Combine(this RuleGroupData condition)
    {
        return TestBase.Combine(new[] { condition }, RuleCombinationOperator.None);
    }

    public static IReadOnlyCollection<RuleDto> Combine(this RuleActionData action)
    {
        return TestBase.Combine(new List<RuleGroupData>(), RuleCombinationOperator.True, action);
    }
}