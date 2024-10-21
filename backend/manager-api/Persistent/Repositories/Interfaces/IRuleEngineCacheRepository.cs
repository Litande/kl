using Plat4Me.DialClientApi.Application.Enums;

namespace Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;

public interface IRuleEngineCacheRepository
{
    Task<string> GetConditions(long clientId, RuleGroupTypes ruleType, CancellationToken ct = default);
    Task<string> GetActions(long clientId, RuleGroupTypes ruleType, CancellationToken ct = default);
    void ResetCache();
}
