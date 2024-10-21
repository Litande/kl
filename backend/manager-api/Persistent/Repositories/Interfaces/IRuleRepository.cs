using Plat4Me.DialClientApi.Persistent.Entities;
using Plat4Me.DialClientApi.Persistent.Entities.Projections;

namespace Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;

public interface IRuleRepository
{
    Task<Rule?> GetById(long clientId, long ruleId, CancellationToken ct = default);
    Task<Rule> AddRule(Rule rule, CancellationToken ct = default);
    Task SaveChanges(CancellationToken ct = default);
    Task<IEnumerable<StatusRuleProjection>> GetStatusRules(long clientId, CancellationToken ct = default);
    Task UpdateStatusRules(long clientId, IEnumerable<StatusRuleProjection> rules, CancellationToken ct = default);
    Task<bool> Delete(long ruleId, CancellationToken ct = default);
}
