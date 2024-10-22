using KL.Manager.API.Application.Enums;
using KL.Manager.API.Application.Models;

namespace KL.Manager.API.Persistent.Clients;

public interface IRuleEngineClient
{
    Task<OperationResponse?> ValidateRules(long clientId, RuleGroupTypes ruleType, string rules, CancellationToken ct = default);
    Task<Dictionary<RuleGroupTypes, string>> GetConditions(long clientId, CancellationToken ct = default);
    Task<Dictionary<RuleGroupTypes, string>> GetActions(long clientId, CancellationToken ct = default);
}
