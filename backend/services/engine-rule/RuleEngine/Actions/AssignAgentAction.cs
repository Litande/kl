using Plat4Me.DialRuleEngine.Application.Enums;
using Plat4Me.DialRuleEngine.Application.Extensions;
using Plat4Me.DialRuleEngine.Application.Models;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Contracts;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Enums;

namespace Plat4Me.DialRuleEngine.Application.RuleEngine.Actions;

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