using Plat4Me.DialClientApi.Application.Enums;
using Plat4Me.DialClientApi.Persistent.Entities;

namespace Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;

public interface IRuleGroupRepository
{
    Task<RuleGroup?> GetById(long currentClientId, long ruleGroupId, CancellationToken ct = default);
    Task<IEnumerable<RuleGroup>> GetRuleGroups(long currentClientId, RuleGroupTypes ruleType, CancellationToken ct = default);
    Task<RuleGroup> AddRuleGroup(RuleGroup ruleGroup, CancellationToken ct = default);
    Task<bool> Delete(long groupId, CancellationToken ct = default);
    Task SaveChanges(CancellationToken ct = default);
}
