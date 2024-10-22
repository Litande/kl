using KL.Manager.API.Application.Enums;
using KL.Manager.API.Persistent.Entities;

namespace KL.Manager.API.Persistent.Seed;

public static class SeedRule
{
    public static void Seed(DialDbContext context, string name, long ruleGroupId, string rule)
    {
        AddRuleIfNeed(context, name, ruleGroupId, rule);
    }

    private static void AddRuleIfNeed(DialDbContext context, string name, long ruleGroupId, string rule)
    {
        var entity = context.Rules
            .FirstOrDefault(r => r.Name == name
                                 && r.RuleGroupId == ruleGroupId);
        if (entity is not null) return;

        var entityRule = new Rule
        {
            RuleGroupId = ruleGroupId,
            Name = name,
            Status = StatusTypes.Enable,
            Rules = rule
        };

        context.Rules.Add(entityRule);
        context.SaveChanges();
    }
}
