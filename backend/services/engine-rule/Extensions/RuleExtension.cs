using Plat4Me.DialRuleEngine.Application.Models;
using Plat4Me.DialRuleEngine.Application.Models.Entities;

namespace Plat4Me.DialRuleEngine.Application.Common.Extensions;

public static class RuleExtension
{
    public static RuleDto ToResponse(this Rule rule)
        => new(rule.QueueId, rule.Name, rule.Rules, rule.Ordinal);
}