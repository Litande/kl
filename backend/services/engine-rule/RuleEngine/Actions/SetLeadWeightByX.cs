using Plat4Me.DialRuleEngine.Application.Enums;
using Plat4Me.DialRuleEngine.Application.Extensions;
using Plat4Me.DialRuleEngine.Application.Models;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Contracts;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Enums;

namespace Plat4Me.DialRuleEngine.Application.RuleEngine.Actions;

public class SetLeadWeightByXAction : RuleActionBase
{
    private static readonly IReadOnlyCollection<ActionOperation> ActionOperations = new[]
    {
        ActionOperation.Set, ActionOperation.Increase, ActionOperation.Decrease
    };

    public SetLeadWeightByXAction() : base(
        new RuleGroupTypes[]
        {
            RuleGroupTypes.LeadScoring
        },
        new RuleActionDescription(
            RulesAction.SetLeadWeightByX.ToString(),
            RulesAction.SetLeadWeightByX.ToDescription(),
            ActionOperationList(ActionOperations),
            new RuleFieldDescription[]
            {
                new(1, RuleValueType.Integer.ToDescription(), "weight", Enumerable.Empty<LabelValue>(), true)
            }
        )
    )
    { }

    public override Task<IRuleActionExecutor> Create(RuleActionData data)
    {
        ValidateFields(data);

        if (data.ActionOperation is null
            || !ActionOperations.Contains(data.ActionOperation.Value))
            throw new ArgumentException("Invalid action operation", data.Name);

        var param1Value = ParseLong(data.Fields![0], data.Name);

        var result = new SetLeadWeightByXExecutor(param1Value, data.ActionOperation.Value);
        return Task.FromResult<IRuleActionExecutor>(result);
    }

    protected class SetLeadWeightByXExecutor : IRuleActionExecutor
    {
        private readonly long _value;
        private readonly ActionOperation _operation;

        public SetLeadWeightByXExecutor(long value, ActionOperation operation)
        {
            _value = value;
            _operation = operation;
        }

        public Task Process(object target)
        {
            if (target is not TrackedLead lead)
                throw new ArgumentException("Expecting non null TrackedLead");

            switch (_operation)
            {
                case ActionOperation.Set:
                    lead.Score = _value;
                    break;
                case ActionOperation.Increase:
                    lead.Score += _value;
                    break;
                case ActionOperation.Decrease:
                    lead.Score -= _value;
                    break;
            }
            return Task.CompletedTask;
        }
    }
}
