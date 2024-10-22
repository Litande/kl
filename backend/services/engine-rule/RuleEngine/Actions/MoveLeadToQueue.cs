using KL.Engine.Rule.Enums;
using KL.Engine.Rule.Extensions;
using KL.Engine.Rule.Models;
using KL.Engine.Rule.RuleEngine.Contracts;
using KL.Engine.Rule.RuleEngine.Enums;

namespace KL.Engine.Rule.RuleEngine.Actions;

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
