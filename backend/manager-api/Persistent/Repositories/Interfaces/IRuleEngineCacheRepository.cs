using KL.Manager.API.Application.Enums;

namespace KL.Manager.API.Persistent.Repositories.Interfaces;

public interface IRuleEngineCacheRepository
{
    Task<string> GetConditions(long clientId, RuleGroupTypes ruleType, CancellationToken ct = default);
    Task<string> GetActions(long clientId, RuleGroupTypes ruleType, CancellationToken ct = default);
    void ResetCache();
}
