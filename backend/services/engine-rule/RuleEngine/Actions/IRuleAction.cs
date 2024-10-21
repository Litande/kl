using Plat4Me.DialRuleEngine.Application.RuleEngine.Contracts;
using Plat4Me.DialRuleEngine.Application.Enums;

namespace Plat4Me.DialRuleEngine.Application.RuleEngine.Actions;

public interface IRuleActionExecutor
{
    Task Process(object target);
}

public interface IRuleAction
{
    string Key { get; }
    RuleActionDescription Description { get; }
    IReadOnlyCollection<RuleGroupTypes> AvailableFor { get; }
    Task<IRuleActionExecutor> Create(RuleActionData data);
}
