using KL.Engine.Rule.Enums;
using KL.Engine.Rule.Extensions;
using KL.Engine.Rule.Models;
using KL.Engine.Rule.RuleEngine.Contracts;
using KL.Engine.Rule.RuleEngine.Enums;

namespace KL.Engine.Rule.RuleEngine.Actions;

public class AssignAgentAction : RuleActionBase
{
    public AssignAgentAction() : base(
        new RuleGroupTypes[]
        {
            RuleGroupTypes.Behavior,
        },
        new RuleActionDescription(
            Name: RulesAction.AssignAgent.ToString(),
            DisplayName: RulesAction.AssignAgent.ToDescription(),
            ActionOperation: null,
            Fields: null
        )
    )
    { }

    public override Task<IRuleActionExecutor> Create(RuleActionData data)
    {
        var result = new AssignAgentExecutor();
        return Task.FromResult<IRuleActionExecutor>(result);
    }

    protected class AssignAgentExecutor : IRuleActionExecutor
    {
        public Task Process(object target)
        {
            if (target is not TrackedLead lead)
                throw new ArgumentException("Expecting non null TrackedLead");

            lead.AssignedAgentId = lead.LastCallAgentId;
            return Task.CompletedTask;
        }
    }
}