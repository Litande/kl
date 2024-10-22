using KL.Engine.Rule.Enums;
using KL.Engine.Rule.RuleEngine.Contracts;

namespace KL.Engine.Rule.RuleEngine.Actions;

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
