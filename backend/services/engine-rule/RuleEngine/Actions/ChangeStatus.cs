using KL.Engine.Rule.Enums;
using KL.Engine.Rule.Extensions;
using KL.Engine.Rule.Models;
using KL.Engine.Rule.RuleEngine.Contracts;
using KL.Engine.Rule.RuleEngine.Enums;

namespace KL.Engine.Rule.RuleEngine.Actions;

public class ChangeStatusAction : RuleActionBase
{
    public ChangeStatusAction() : base(
        new RuleGroupTypes[]
        {
            RuleGroupTypes.Behavior,
            RuleGroupTypes.ApiRules
        },
        new RuleActionDescription(
            RulesAction.ChangeStatus.ToString(),
            RulesAction.ChangeStatus.ToDescription(),
            ActionOperationList(new[] { ActionOperation.To }), //? ActionOperation.Set
            new RuleFieldDescription[]
            {
                new(1, RuleValueType.Select.ToDescription(), "status", LeadStatuses, true)
            }
        )
    )
    { }

    public override Task<IRuleActionExecutor> Create(RuleActionData data)
    {
        ValidateFields(data);

        var param1Value = ParseLeadStatus(data.Fields![0], data.Name);

        if (data.ActionOperation != ActionOperation.To)
            throw new ArgumentException($"{data.Name}: Invalid action operation"); //???

        var result = new ChangeStatusExecutor(param1Value);
        return Task.FromResult<IRuleActionExecutor>(result);
    }

    protected class ChangeStatusExecutor : IRuleActionExecutor
    {
        private readonly LeadStatusTypes _targetStatus;

        public ChangeStatusExecutor(LeadStatusTypes targetStatus)
        {
            _targetStatus = targetStatus;
        }

        public Task Process(object target)
        {
            if (target is not TrackedLead lead)
                throw new ArgumentException("Expecting non null TrackedLead");

            lead.Status = _targetStatus;
            return Task.CompletedTask;
        }
    }
}
