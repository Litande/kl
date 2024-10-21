using Plat4Me.DialRuleEngine.Application.Enums;
using Plat4Me.DialRuleEngine.Application.Extensions;
using Plat4Me.DialRuleEngine.Application.Models;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Contracts;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Enums;

namespace Plat4Me.DialRuleEngine.Application.RuleEngine.Actions;

public class MoveLeadToQueueAction : RuleActionBase
{
    public MoveLeadToQueueAction() : base(
        new RuleGroupTypes[]
        {
            RuleGroupTypes.ForwardRules
        },
        new RuleActionDescription(
            RulesAction.MoveLeadToQueue.ToString(),
            RulesAction.MoveLeadToQueue.ToDescription(),
            ActionOperationList(new[] { ActionOperation.To }), //? ActionOperation.Set
            new RuleFieldDescription[]
            {
                new(1, RuleValueType.Select.ToDescription(), "QueueId", Enumerable.Empty<LabelValue>(), true)
            }
        )
    )
    { }

    public override Task<IRuleActionExecutor> Create(RuleActionData data)
    {
        ValidateFields(data);

        if (data.ActionOperation != ActionOperation.To)
            throw new ArgumentException($"{data.Name}: Invalid action operation"); //???

        var param1Value = ParseLong(data.Fields![0], data.Name, RuleValueType.Select);

        var result = new MoveLeadToQueueExecutor(param1Value);
        return Task.FromResult<IRuleActionExecutor>(result);
    }
    protected class MoveLeadToQueueExecutor : IRuleActionExecutor
    {
        private readonly long _value;

        public MoveLeadToQueueExecutor(long value)
        {
            _value = value;
        }

        public Task Process(object target)
        {
            if (target is not TrackedLead lead)
                throw new ArgumentException("Expecting non null TrackedLead");

            lead.LeadQueueId = _value;
            return Task.CompletedTask;

        }
    }
}
