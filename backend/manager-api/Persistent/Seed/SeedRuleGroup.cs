using KL.Manager.API.Application.Enums;
using KL.Manager.API.Persistent.Entities;

namespace KL.Manager.API.Persistent.Seed;

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