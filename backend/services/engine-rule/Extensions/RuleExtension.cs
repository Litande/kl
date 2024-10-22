using KL.Engine.Rule.Models;

namespace KL.Engine.Rule.Extensions;

public static class RuleExtension
{
    public static RuleDto ToResponse(this Models.Entities.Rule rule)
        => new(rule.QueueId, rule.Name, rule.Rules, rule.Ordinal);
}