using Plat4Me.DialClientApi.Application.Enums;
using Plat4Me.DialClientApi.Persistent.Entities;

namespace Plat4Me.DialClientApi.Persistent.Seed;

public class SeedRuleGroup
{
    public static long Seed(DialDbContext context, string ruleGroupName, RuleGroupTypes groupType, long clientId)
    {
        var ruleGroupId = AddRuleGroupsIfNeed(context, ruleGroupName, groupType, clientId);
        return ruleGroupId;
    }

    private static long AddRuleGroupsIfNeed(
        DialDbContext context,
        string ruleGroupName,
        RuleGroupTypes groupType,
        long clientId)
    {
        var entity = context.GroupRules
            .FirstOrDefault(r => r.Name == ruleGroupName
                                 && r.GroupType == groupType
                                 && r.ClientId == clientId);

        if (entity is not null) return entity.Id;

        var ruleGroupEntity = new RuleGroup
        {
            Name = ruleGroupName,
            Status = StatusTypes.Enable,
            GroupType = groupType,
            ClientId = clientId
        };

        context.GroupRules.Add(ruleGroupEntity);
        context.SaveChanges();

        return ruleGroupEntity.Id;
    }
}