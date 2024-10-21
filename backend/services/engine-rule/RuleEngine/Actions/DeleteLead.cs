using Plat4Me.DialRuleEngine.Application.Enums;
using Plat4Me.DialRuleEngine.Application.Extensions;
using Plat4Me.DialRuleEngine.Application.Models;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Contracts;
using Plat4Me.DialRuleEngine.Application.RuleEngine.Enums;

namespace Plat4Me.DialRuleEngine.Application.RuleEngine.Actions;

public class DeleteLeadAction : RuleActionBase
{
    public DeleteLeadAction() : base(
        new RuleGroupTypes[]
        {
            RuleGroupTypes.Behavior
        },
        new RuleActionDescription(
            RulesAction.DeleteLead.ToString(),
            RulesAction.DeleteLead.ToDescription(),
            null,
            null
        )
    )
    { }

    public override Task<IRuleActionExecutor> Create(RuleActionData data)
    {
        var result = new DeleteLeadExecutor();
        return Task.FromResult<IRuleActionExecutor>(result);
    }

    protected class DeleteLeadExecutor : IRuleActionExecutor
    {
        public Task Process(object target)
        {
            if (target is not TrackedLead lead)
                throw new ArgumentException("Expecting non null TrackedLead");

            lead.DeletedOn = DateTimeOffset.UtcNow;

            return Task.CompletedTask;
        }
    }
}
